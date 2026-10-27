using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Orchestrates URL safety checks combining URLhaus and Google Safe Browsing.
/// </summary>
public class UrlSafetyOrchestrator : IUrlSafetyOrchestrator
{
    private readonly IUrlhausService _urlhausService;
    private readonly IGoogleSafeBrowsingService _safeBrowsingService;
    private readonly ICloudflareUrlScannerService _cloudflareScanner;
    private readonly IUrlRedirectResolver _urlRedirectResolver;
    private readonly IUrlRepository _urlRepository;
    private readonly IOptions<SafetyRecheckOptions> _recheckOptions;
    private readonly ILogger<UrlSafetyOrchestrator> _logger;

    public UrlSafetyOrchestrator(
        IUrlhausService urlhausService,
        IGoogleSafeBrowsingService safeBrowsingService,
        ICloudflareUrlScannerService cloudflareScanner,
        IUrlRedirectResolver urlRedirectResolver,
        IUrlRepository urlRepository,
        IOptions<SafetyRecheckOptions> recheckOptions,
        ILogger<UrlSafetyOrchestrator> logger)
    {
        _urlhausService = urlhausService;
        _safeBrowsingService = safeBrowsingService;
        _cloudflareScanner = cloudflareScanner;
        _urlRedirectResolver = urlRedirectResolver;
        _urlRepository = urlRepository;
        _recheckOptions = recheckOptions;
        _logger = logger;
    }

    public UrlSafetyOrchestrator(
        IUrlhausService urlhausService,
        IGoogleSafeBrowsingService safeBrowsingService,
        ICloudflareUrlScannerService cloudflareScanner,
        IUrlRepository urlRepository,
        IOptions<SafetyRecheckOptions> recheckOptions,
        ILogger<UrlSafetyOrchestrator> logger)
        : this(urlhausService, safeBrowsingService, cloudflareScanner, null!, urlRepository, recheckOptions, logger)
    {
    }

    public async Task<UrlSafetyCheckResult> QuickCheckAsync(string url, CancellationToken cancellationToken = default)
    {
        return await QuickCheckChainAsync(new[] { url }, cancellationToken);
    }

    public async Task<UrlSafetyCheckResult> QuickCheckChainAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        var urlList = urls.Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (!urlList.Any())
        {
            return UrlSafetyCheckResult.Safe();
        }

        // Run independent security providers concurrently
        Task<UrlhausResult>? urlhausTask = null;
        Task<SafeBrowsingResult>? gsbTask = null;
        var cfTasks = new List<Task<CloudflareScanResult>>();

        try
        {
            urlhausTask = _urlhausService.CheckUrlsAsync(urlList, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronous error invoking URLhaus check.");
            return UrlSafetyCheckResult.ApiError($"URLhaus verification error: {ex.Message}");
        }

        try
        {
            gsbTask = _safeBrowsingService.CheckUrlsAsync(urlList, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronous error invoking Google Safe Browsing check.");
            return UrlSafetyCheckResult.ApiError($"Google Safe Browsing verification error: {ex.Message}");
        }

        try
        {
            foreach (var u in urlList)
            {
                cfTasks.Add(_cloudflareScanner.QuickCheckUrlAsync(u, cancellationToken));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronous error invoking Cloudflare scanner.");
            return UrlSafetyCheckResult.ApiError($"Cloudflare scanner verification error: {ex.Message}");
        }

        try
        {
            await Task.WhenAll(new Task[] { urlhausTask, gsbTask }.Concat(cfTasks));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error awaiting concurrent security checks.");
        }

        // 1. Evaluate URLhaus (abuse.ch) verdict
        if (urlhausTask.IsCompletedSuccessfully)
        {
            var urlhausResult = urlhausTask.Result;
            if (!urlhausResult.IsSafe)
            {
                _logger.LogWarning("URLhaus blocked URL: {Url}, Threat: {Threat}", urlhausResult.FlaggedUrl, urlhausResult.ThreatType);
                return UrlSafetyCheckResult.Unsafe("URLhaus", urlhausResult.ThreatType ?? "Malware", urlhausResult.FlaggedUrl ?? urlList.First(), urlhausResult.Description ?? "Identified as malware by URLhaus.");
            }
        }
        else if (urlhausTask.IsFaulted)
        {
            _logger.LogError(urlhausTask.Exception, "Error while checking URLs against URLhaus.");
            return UrlSafetyCheckResult.ApiError($"URLhaus verification failed: {urlhausTask.Exception?.InnerException?.Message ?? "faulted"}");
        }

        // 2. Evaluate Google Safe Browsing verdict
        if (gsbTask.IsCompletedSuccessfully)
        {
            var gsbResult = gsbTask.Result;
            if (!gsbResult.IsSafe)
            {
                _logger.LogWarning("Google Safe Browsing blocked URL: {Url}, Threat: {Threat}", gsbResult.FlaggedUrl, gsbResult.ThreatType);
                return UrlSafetyCheckResult.Unsafe("GoogleSafeBrowsing", gsbResult.ThreatType ?? "UNSAFE", gsbResult.FlaggedUrl ?? urlList.First(), gsbResult.Description ?? "Identified as unsafe by Google Safe Browsing.");
            }
        }
        else if (gsbTask.IsFaulted)
        {
            _logger.LogError(gsbTask.Exception, "Error while checking URLs against Google Safe Browsing.");
            return UrlSafetyCheckResult.ApiError($"Google Safe Browsing verification failed: {gsbTask.Exception?.InnerException?.Message ?? "faulted"}");
        }

        // 3. Evaluate Cloudflare URL Scanner verdicts
        foreach (var cfTask in cfTasks)
        {
            if (cfTask.IsCompletedSuccessfully)
            {
                var cfResult = cfTask.Result;
                // If Cloudflare returned null or had an API error, bypass Cloudflare and depend on other verifications
                if (cfResult == null || cfResult.ErrorOccurred)
                {
                    _logger.LogWarning("Cloudflare URL Scanner encountered an API issue or error ({Reason}). Bypassing Cloudflare and depending on other verifications.",
                        cfResult?.Description ?? "Null response");
                    continue;
                }

                if (cfResult.IsMalicious)
                {
                    _logger.LogWarning("Cloudflare URL Scanner blocked URL. Categories: {Categories}", string.Join(", ", cfResult.Categories));
                    return UrlSafetyCheckResult.Unsafe(
                        "Cloudflare URL Scanner",
                        string.Join("_", cfResult.Categories),
                        urlList.First(),
                        cfResult.Description ?? "Identified as malicious by Cloudflare URL Scanner.");
                }

                if (cfResult.IsPending)
                {
                    _logger.LogWarning("Cloudflare URL Scanner verdict is still pending after maximum wait.");
                    return UrlSafetyCheckResult.Pending(
                        "Cloudflare URL Scanner",
                        cfResult.Description ?? "Security verification for this URL is still processing. Please try again in a few moments.");
                }
            }
            else if (cfTask.IsFaulted)
            {
                _logger.LogWarning(cfTask.Exception, "Error while checking URLs against Cloudflare URL Scanner. Depending on other verification providers.");
            }
        }

        // Capture scan UUID of the final destination URL in the chain
        var destinationCfTask = cfTasks.LastOrDefault();
        string? destinationScanUuid = destinationCfTask?.IsCompletedSuccessfully == true ? destinationCfTask.Result?.ScanUuid : null;

        return UrlSafetyCheckResult.Safe(destinationScanUuid);
    }

    public async Task<UrlValidationPipelineResult> ValidateFullPipelineAsync(string url, CancellationToken cancellationToken = default)
    {
        if (_urlRedirectResolver == null)
        {
            throw new InvalidOperationException("UrlRedirectResolver is not configured on this orchestrator instance.");
        }

        // 1. Resolve redirect chain (enforcing maximum 2 redirects)
        var redirectResult = await _urlRedirectResolver.ResolveChainAsync(url, cancellationToken);
        if (!redirectResult.Success)
        {
            var isLimitExceeded = redirectResult.ErrorMessage?.Contains("redirect", StringComparison.OrdinalIgnoreCase) == true
                && (redirectResult.ErrorMessage.Contains("maximum", StringComparison.OrdinalIgnoreCase) || redirectResult.ErrorMessage.Contains("too many", StringComparison.OrdinalIgnoreCase));
            return isLimitExceeded
                ? UrlValidationPipelineResult.RedirectLimitExceeded(
                    redirectResult.ErrorMessage!,
                    redirectResult.RedirectChain)
                : UrlValidationPipelineResult.Failure(
                    redirectResult.ErrorMessage ?? "Failed to validate redirect chain.",
                    redirectResult.RedirectChain);
        }

        var urlsToCheck = redirectResult.RedirectChain.Count > 0
            ? redirectResult.RedirectChain
            : new List<string> { url };

        // 2. Concurrently run threat intelligence checks across all discovered hops
        var safetyCheck = await QuickCheckChainAsync(urlsToCheck, cancellationToken);
        if (!safetyCheck.IsSafe)
        {
            if (safetyCheck.IsPending)
            {
                return UrlValidationPipelineResult.InconclusiveOrPending(
                    safetyCheck.Description ?? "Security verification for this URL is still processing. Please try again in a few moments.",
                    urlsToCheck);
            }

            if (safetyCheck.WasApiError)
            {
                return UrlValidationPipelineResult.ApiError(
                    safetyCheck.Description ?? "Security verification could not be completed at this time. Please try again.",
                    urlsToCheck);
            }

            return UrlValidationPipelineResult.ThreatDetected(
                safetyCheck.ThreatSource ?? "ThreatIntelligence",
                safetyCheck.ThreatType ?? "Malicious",
                safetyCheck.FlaggedUrl ?? url,
                safetyCheck.Description ?? "The destination URL or a redirect hop has been identified as unsafe.",
                urlsToCheck);
        }

        var finalUrl = redirectResult.FinalUrl ?? urlsToCheck.Last();
        return UrlValidationPipelineResult.Success(finalUrl, urlsToCheck, safetyCheck.ScanUuid);
    }

    public async Task<UrlSafetyCheckResult> CheckRedirectSafetyAsync(UrlMapping url, CancellationToken cancellationToken = default)
    {
        var options = _recheckOptions.Value;
        if (!options.EnableRedirectTimeRecheck)
        {
            return UrlSafetyCheckResult.Safe();
        }

        var needsRecheck = url.LastSafetyCheckAt == null
            || (DateTime.UtcNow - url.LastSafetyCheckAt.Value).TotalSeconds > options.CacheWindowSeconds;

        if (!needsRecheck)
        {
            return UrlSafetyCheckResult.Safe();
        }

        var safetyResult = await QuickCheckAsync(url.LongUrl, cancellationToken);
        if (!safetyResult.IsSafe)
        {
            await _urlRepository.FlagUrlAsync(url.Id, $"{safetyResult.ThreatSource}:{safetyResult.ThreatType}", cancellationToken);
            url.IsFlagged = true;
            url.FlaggedReason = $"{safetyResult.ThreatSource}:{safetyResult.ThreatType}";
            url.FlaggedAt = DateTime.UtcNow;
            url.LastSafetyCheckAt = DateTime.UtcNow;
            return safetyResult;
        }

        await _urlRepository.UpdateLastSafetyCheckAsync(url.Id, cancellationToken);
        url.LastSafetyCheckAt = DateTime.UtcNow;
        return UrlSafetyCheckResult.Safe();
    }
}
