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
    private readonly IUrlRepository _urlRepository;
    private readonly IOptions<SafetyRecheckOptions> _recheckOptions;
    private readonly ILogger<UrlSafetyOrchestrator> _logger;

    public UrlSafetyOrchestrator(
        IUrlhausService urlhausService,
        IGoogleSafeBrowsingService safeBrowsingService,
        ICloudflareUrlScannerService cloudflareScanner,
        IUrlRepository urlRepository,
        IOptions<SafetyRecheckOptions> recheckOptions,
        ILogger<UrlSafetyOrchestrator> logger)
    {
        _urlhausService = urlhausService;
        _safeBrowsingService = safeBrowsingService;
        _cloudflareScanner = cloudflareScanner;
        _urlRepository = urlRepository;
        _recheckOptions = recheckOptions;
        _logger = logger;
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

        // 1. Fast path: URLhaus (0ms, local in-memory HashSet lookup)
        try
        {
            var urlhausResult = await _urlhausService.CheckUrlsAsync(urlList, cancellationToken);
            if (!urlhausResult.IsSafe)
            {
                _logger.LogWarning("URLhaus blocked URL: {Url}, Threat: {Threat}", urlhausResult.FlaggedUrl, urlhausResult.ThreatType);
                return UrlSafetyCheckResult.Unsafe("urlhaus", urlhausResult.ThreatType ?? "malware", urlhausResult.FlaggedUrl ?? urlList.First(), urlhausResult.Description ?? "Identified as malware by URLhaus.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking URLs against URLhaus.");
        }

        // 2. Google Safe Browsing API check
        try
        {
            var gsbResult = await _safeBrowsingService.CheckUrlsAsync(urlList, cancellationToken);
            if (!gsbResult.IsSafe)
            {
                _logger.LogWarning("Google Safe Browsing blocked URL: {Url}, Threat: {Threat}", gsbResult.FlaggedUrl, gsbResult.ThreatType);
                return UrlSafetyCheckResult.Unsafe("google_safe_browsing", gsbResult.ThreatType ?? "UNSAFE", gsbResult.FlaggedUrl ?? urlList.First(), gsbResult.Description ?? "Identified as unsafe by Google Safe Browsing.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking URLs against Google Safe Browsing.");
            return UrlSafetyCheckResult.ApiError("Google Safe Browsing verification failed due to error.");
        }

        // 3. Cloudflare URL Scanner check
        try
        {
            foreach (var u in urlList)
            {
                var cfResult = await _cloudflareScanner.QuickCheckUrlAsync(u, cancellationToken);
                if (cfResult.IsMalicious)
                {
                    _logger.LogWarning("Cloudflare URL Scanner blocked URL: {Url}, Categories: {Categories}",
                        u, string.Join(", ", cfResult.Categories));
                    return UrlSafetyCheckResult.Unsafe(
                        "cloudflare_url_scanner",
                        string.Join("_", cfResult.Categories),
                        u,
                        cfResult.Description ?? "Identified as malicious by Cloudflare URL Scanner.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking URLs against Cloudflare URL Scanner.");
        }

        return UrlSafetyCheckResult.Safe();
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
