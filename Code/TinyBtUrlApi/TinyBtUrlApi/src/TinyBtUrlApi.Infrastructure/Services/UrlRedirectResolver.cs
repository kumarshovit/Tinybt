using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Service that traces HTTP redirect chains to uncover downstream landing destinations and protect against cloaking.
/// </summary>
public class UrlRedirectResolver : IUrlRedirectResolver
{
    private readonly HttpClient _httpClient;
    private readonly IUrlFormatValidator _formatValidator;
    private readonly UrlSecurityOptions _options;
    private readonly ILogger<UrlRedirectResolver> _logger;

    private const string UserAgent = "Mozilla/5.0 (compatible; TinyBtBot/1.0; +https://link.bt)";

    public UrlRedirectResolver(
        HttpClient httpClient,
        IUrlFormatValidator formatValidator,
        IOptions<UrlSecurityOptions> options,
        ILogger<UrlRedirectResolver> logger)
    {
        _httpClient = httpClient;
        _formatValidator = formatValidator;
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
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromMilliseconds(_options.RedirectTimeoutMilliseconds));

            HttpResponseMessage? response = null;
            try
            {
                // 1. Try HEAD request first for performance
                using var headRequest = new HttpRequestMessage(HttpMethod.Head, currentUrl);
                headRequest.Headers.TryAddWithoutValidation("User-Agent", UserAgent);

                response = await _httpClient.SendAsync(headRequest, HttpCompletionOption.ResponseHeadersRead, cts.Token);

                // Fallback to GET if HEAD is not supported by target server
                if (response.StatusCode == HttpStatusCode.MethodNotAllowed ||
                    response.StatusCode == HttpStatusCode.NotImplemented)
                {
                    response.Dispose();
                    using var getRequest = new HttpRequestMessage(HttpMethod.Get, currentUrl);
                    getRequest.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
                    response = await _httpClient.SendAsync(getRequest, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                }
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Redirect check timed out for hop URL: {Url}", currentUrl);
                // Timeout on hop - return the chain collected so far
                break;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "HTTP request failed during redirect resolution for URL: {Url}", currentUrl);
                // Cannot follow further - return the chain collected so far
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during redirect resolution for URL: {Url}", currentUrl);
                break;
            }

            using (response)
            {
                if (response == null) break;

                var statusCode = (int)response.StatusCode;

                // Check for HTTP 3xx Redirection (301, 302, 303, 307, 308)
                if (statusCode is >= 300 and <= 399)
                {
                    var location = response.Headers.Location;
                    if (location == null)
                    {
                        // 3xx without Location header
                        break;
                    }

                    // Resolve relative URLs to absolute
                    string nextUrl;
                    if (location.IsAbsoluteUri)
                    {
                        nextUrl = location.ToString();
                    }
                    else
                    {
                        if (!Uri.TryCreate(new Uri(currentUrl), location, out var resolvedUri))
                        {
                            return UrlRedirectResolutionResult.CreateFailure(
                                $"Invalid redirect location '{location}' from '{currentUrl}'.", chain);
                        }
                        nextUrl = resolvedUri.ToString();
                    }

                    // 2. Validate format and SSRF safety of the hop
                    var formatCheck = _formatValidator.ValidateFormat(nextUrl);
                    if (!formatCheck.Success)
                    {
                        _logger.LogWarning("Redirect hop to '{NextUrl}' failed validation: {Message}", nextUrl, formatCheck.Message);
                        return UrlRedirectResolutionResult.CreateFailure(
                            $"Redirect hop to '{nextUrl}' is invalid or blocked: {formatCheck.Message}", chain);
                    }

                    var normalizedNextUrl = formatCheck.NormalizedUrl ?? nextUrl;

                    // 3. Check for circular loops
                    if (visited.Contains(normalizedNextUrl))
                    {
                        _logger.LogWarning("Circular redirect loop detected: {NextUrl}", normalizedNextUrl);
                        return UrlRedirectResolutionResult.CreateFailure(
                            "Circular redirect loop detected in URL.", chain);
                    }

                    visited.Add(normalizedNextUrl);
                    chain.Add(normalizedNextUrl);
                    currentUrl = normalizedNextUrl;

                    // 4. Check if max hops exceeded
                    if (hop == maxHops - 1)
                    {
                        _logger.LogWarning("Exceeded maximum redirect hops ({MaxHops}) for URL: {Url}", maxHops, initialUrl);
                        return UrlRedirectResolutionResult.CreateFailure(
                            $"URL exceeded the maximum allowed redirect limit of {maxHops} hops.", chain);
                    }
                }
                else
                {
                    // Terminal response reached (e.g. 200 OK, 404, etc.)
                    break;
                }
            }
        }

        return UrlRedirectResolutionResult.CreateSuccess(currentUrl, chain);
    }
}
