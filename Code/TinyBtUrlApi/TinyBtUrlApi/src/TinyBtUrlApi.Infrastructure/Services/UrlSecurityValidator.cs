using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Services;

public class UrlSecurityValidator : IUrlSecurityValidator
{
    private readonly IUrlFormatValidator _formatValidator;
    private readonly IDnsLookupService _dnsService;
    //private readonly IHttpsValidationService _httpsService;
    private readonly UrlSecurityOptions _options;
    private readonly ILogger<UrlSecurityValidator> _logger;

    public UrlSecurityValidator(
        IUrlFormatValidator formatValidator,
        IDnsLookupService dnsService,
        //IHttpsValidationService httpsService,
        IOptions<UrlSecurityOptions> options,
        ILogger<UrlSecurityValidator> logger)
    {
        _formatValidator = formatValidator;
        _dnsService = dnsService;
        //_httpsService = httpsService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<UrlSecurityResult> ValidateUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        // 1. Format validation
        var formatResult = _formatValidator.ValidateFormat(url);
        if (!formatResult.Success)
        {
            return formatResult;
        }

        var normalizedUrl = formatResult.NormalizedUrl;

        // Parse URI again to get host and scheme reliably
        if (!Uri.TryCreate(normalizedUrl, UriKind.Absolute, out var uri))
        {
            return UrlSecurityResult.CreateFailure("Invalid URL format.");
        }

        var host = uri.Host;

        // 2. DNS Validation
        if (_options.EnableDnsValidation)
        {
            try
            {
                using var dnsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                dnsCts.CancelAfter(TimeSpan.FromMilliseconds(_options.DnsTimeoutMilliseconds));

                var hasValidDns = await _dnsService.HasValidDnsRecordsAsync(host, dnsCts.Token);
                if (!hasValidDns)
                {
                    return UrlSecurityResult.CreateFailure($"The destination domain '{host}' could not be resolved or lacks valid DNS records.");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DNS lookup timed out for domain: {Host}", host);
                return UrlSecurityResult.CreateFailure($"DNS resolution for domain '{host}' timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during DNS lookup for {Host}", host);
                return UrlSecurityResult.CreateFailure("An error occurred while resolving the domain.");
            }
        }

        // 3. HTTPS Preference
        //if (_options.PreferHttps && uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        //{
        //    try
        //    {
        //        using var httpsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        //        httpsCts.CancelAfter(TimeSpan.FromMilliseconds(_options.HttpsTimeoutMilliseconds));

        //        var isHttpsSupported = await _httpsService.IsHttpsSupportedAsync(host, httpsCts.Token);
        //        if (isHttpsSupported)
        //        {
        //            var builder = new UriBuilder(uri)
        //            {
        //                Scheme = Uri.UriSchemeHttps,
        //                Port = uri.IsDefaultPort ? -1 : uri.Port // Default HTTPS port is 443
        //            };

        //            if (builder.Port == 80) // if explicitly HTTP port, remove it for HTTPS
        //                builder.Port = -1;

        //            normalizedUrl = builder.Uri.ToString();
        //            _logger.LogInformation("Upgraded URL to HTTPS: {OriginalUrl} -> {NormalizedUrl}", url, normalizedUrl);
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        _logger.LogWarning("HTTPS check timed out for domain: {Host}. Proceeding with HTTP.", host);
        //        // Proceed with HTTP as it timed out
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error during HTTPS check for {Host}. Proceeding with HTTP.", host);
        //        // Non-fatal, keep HTTP
        //    }
        //}

        return UrlSecurityResult.CreateSuccess(normalizedUrl);
    }
}
