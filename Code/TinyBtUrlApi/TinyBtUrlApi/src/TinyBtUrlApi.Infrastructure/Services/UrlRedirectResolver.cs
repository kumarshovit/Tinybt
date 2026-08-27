using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Service that traces HTTP and HTML meta redirect chains to uncover downstream landing destinations and protect against cloaking.
/// </summary>
public class UrlRedirectResolver : IUrlRedirectResolver
{
    private readonly HttpClient _httpClient;
    private readonly IUrlSecurityValidator _urlSecurityValidator;
    private readonly UrlSecurityOptions _options;
    private readonly ILogger<UrlRedirectResolver> _logger;

    private const string BrowserUserAgent = 
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";
    private const string BrowserAcceptHeader = 
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8";

    private static readonly Regex MetaRefreshRegex = new(
        @"<meta\s+[^>]*http-equiv\s*=\s*[""']?refresh[""']?[^>]*content\s*=\s*[""']?\d+;\s*url\s*=\s*([^""'>\s]+)[""']?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public UrlRedirectResolver(
        HttpClient httpClient,
        IUrlSecurityValidator urlSecurityValidator,
        IOptions<UrlSecurityOptions> options,
        ILogger<UrlRedirectResolver> logger)
    {
        _httpClient = httpClient;
        _urlSecurityValidator = urlSecurityValidator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<UrlRedirectResolutionResult> ResolveChainAsync(
        string initialUrl,
        CancellationToken cancellationToken = default)
    {
        if (!_options.EnableRedirectChainValidation)
        {
            return UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl });
        }

        var chain = new List<string> { initialUrl };
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { initialUrl };
        var currentUrl = initialUrl;
        var maxHops = Math.Max(1, _options.MaxRedirectHops);

        for (int hop = 0; hop < maxHops; hop++)
        {
            // 1. Validate security & DNS before connecting to currentUrl
            var securityCheck = await _urlSecurityValidator.ValidateUrlAsync(currentUrl, cancellationToken);
            if (!securityCheck.Success)
            {
                _logger.LogWarning("URL/Hop '{Url}' failed security validation: {Message}", currentUrl, securityCheck.Message);
                return UrlRedirectResolutionResult.CreateFailure(
                    $"URL or redirect destination '{currentUrl}' is unsafe: {securityCheck.Message}", chain);
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(_options.RedirectTimeoutMilliseconds));

            HttpResponseMessage? response = null;
            try
            {
                // Always use GET with browser headers to defeat cloaking filters (which return 200 to HEAD but 302 to GET)
                using var request = new HttpRequestMessage(HttpMethod.Get, currentUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", BrowserUserAgent);
                request.Headers.TryAddWithoutValidation("Accept", BrowserAcceptHeader);

                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Redirect check timed out for hop URL: {Url}", currentUrl);
                return UrlRedirectResolutionResult.CreateFailure(
                    $"Redirect destination '{currentUrl}' could not be reached (connection timed out).", chain);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "HTTP request failed during redirect resolution for URL: {Url}", currentUrl);
                return UrlRedirectResolutionResult.CreateFailure(
                    $"Redirect destination '{currentUrl}' could not be reached: {ex.Message}", chain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during redirect resolution for URL: {Url}", currentUrl);
                return UrlRedirectResolutionResult.CreateFailure(
                    $"Error resolving destination '{currentUrl}'.", chain);
            }

            using (response)
            {
                if (response == null) break;

                var statusCode = (int)response.StatusCode;
                string? nextUrl = null;

                // Check for HTTP 3xx Redirection (301, 302, 303, 307, 308)
                if (statusCode is >= 300 and <= 399)
                {
                    var location = response.Headers.Location;
                    if (location != null)
                    {
                        nextUrl = location.IsAbsoluteUri 
                            ? location.ToString() 
                            : (Uri.TryCreate(new Uri(currentUrl), location, out var resolved) ? resolved.ToString() : null);

                        if (nextUrl == null)
                        {
                            return UrlRedirectResolutionResult.CreateFailure(
                                $"Invalid redirect location '{location}' from '{currentUrl}'.", chain);
                        }
                    }
                }
                else if (statusCode == 200)
                {
                    // Check for HTML meta refresh redirect (first 4KB of content)
                    try
                    {
                        var contentType = response.Content.Headers.ContentType?.MediaType;
                        if (string.IsNullOrEmpty(contentType) || contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
                        {
                            using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
                            var buffer = new byte[4096];
                            var read = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                            if (read > 0)
                            {
                                var htmlSnippet = System.Text.Encoding.UTF8.GetString(buffer, 0, read);
                                var match = MetaRefreshRegex.Match(htmlSnippet);
                                if (match.Success)
                                {
                                    var metaTarget = match.Groups[1].Value.Trim('\'', '"', ' ');
                                    if (Uri.TryCreate(metaTarget, UriKind.Absolute, out var absUri))
                                    {
                                        nextUrl = absUri.ToString();
                                    }
                                    else if (Uri.TryCreate(new Uri(currentUrl), metaTarget, out var relUri))
                                    {
                                        nextUrl = relUri.ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Non-critical, ignore HTML read errors
                    }
                }

                if (nextUrl != null)
                {
                    // Check for circular loop
                    if (visited.Contains(nextUrl))
                    {
                        _logger.LogWarning("Circular redirect loop detected: {NextUrl}", nextUrl);
                        return UrlRedirectResolutionResult.CreateFailure(
                            "Circular redirect loop detected in URL.", chain);
                    }

                    visited.Add(nextUrl);
                    chain.Add(nextUrl);
                    currentUrl = nextUrl;

                    if (hop == maxHops - 1)
                    {
                        _logger.LogWarning("Exceeded maximum redirect hops ({MaxHops}) for URL: {Url}", maxHops, initialUrl);
                        return UrlRedirectResolutionResult.CreateFailure(
                            $"URL exceeded the maximum allowed redirect limit of {maxHops} hops.", chain);
                    }
                }
                else
                {
                    // Terminal response reached
                    break;
                }
            }
        }

        // Validate the final landing URL as well
        var finalSecurityCheck = await _urlSecurityValidator.ValidateUrlAsync(currentUrl, cancellationToken);
        if (!finalSecurityCheck.Success)
        {
            return UrlRedirectResolutionResult.CreateFailure(
                $"Final destination '{currentUrl}' failed security check: {finalSecurityCheck.Message}", chain);
        }

        return UrlRedirectResolutionResult.CreateSuccess(currentUrl, chain);
    }
}
