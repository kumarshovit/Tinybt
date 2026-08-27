using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Infrastructure.Models.GoogleSafeBrowsing;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Service for verifying URLs against Google Safe Browsing v4 API.
/// </summary>
public class GoogleSafeBrowsingService : IGoogleSafeBrowsingService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleSafeBrowsingOptions _options;
    private readonly ILogger<GoogleSafeBrowsingService> _logger;

    public GoogleSafeBrowsingService(
        HttpClient httpClient,
        IOptions<GoogleSafeBrowsingOptions> options,
        ILogger<GoogleSafeBrowsingService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<SafeBrowsingResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return CheckUrlsAsync(new[] { url }, cancellationToken);
    }

    public async Task<SafeBrowsingResult> CheckUrlsAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        var urlList = urls.Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (!urlList.Any())
        {
            return new SafeBrowsingResult { IsSafe = true };
        }

        try
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("Google Safe Browsing API Key is not configured.");
                return new SafeBrowsingResult { IsSafe = true, Description = "API Key missing; skipped check." };
            }

            var requestUri = $"https://safebrowsing.googleapis.com/v4/threatMatches:find?key={_options.ApiKey}";

            var requestModel = new SafeBrowsingRequest
            {
                Client = new SafeBrowsingClientInfo
                {
                    ClientId = "TinyBtUrlApi",
                    ClientVersion = "1.0.0"
                },
                ThreatInfo = new SafeBrowsingThreatInfo
                {
                    ThreatTypes = GoogleThreatTypes.All,
                    PlatformTypes = new[] { "ANY_PLATFORM" },
                    ThreatEntryTypes = new[] { "URL" },
                    ThreatEntries = urlList.Select(u => new SafeBrowsingWarningEntry { Url = u }).ToArray()
                }
            };

            var response = await _httpClient.PostAsJsonAsync(requestUri, requestModel, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseResult = await response.Content.ReadFromJsonAsync<SafeBrowsingResponse>(cancellationToken: cancellationToken);

            if (responseResult?.Matches != null && responseResult.Matches.Any())
            {
                var match = responseResult.Matches.First();
                var flaggedUrl = match.Threat?.Url ?? urlList.First();
                _logger.LogWarning("URL blocked by Google Safe Browsing. URL: {Url}, ThreatType: {ThreatType}", flaggedUrl, match.ThreatType);
                
                return new SafeBrowsingResult
                {
                    IsSafe = false,
                    ThreatType = match.ThreatType,
                    FlaggedUrl = flaggedUrl,
                    Description = $"The destination URL or a redirect hop ('{flaggedUrl}') has been identified as unsafe ({match.ThreatType})."
                };
            }

            return new SafeBrowsingResult { IsSafe = true };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP Request Error while checking URLs against Google Safe Browsing API.");
            return new SafeBrowsingResult { IsSafe = true, Description = "Failed to validate URL against Safe Browsing API due to network error." };
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogError(ex, "JSON Parsing Error from Google Safe Browsing API response.");
            return new SafeBrowsingResult { IsSafe = true, Description = "Failed to validate URL against Safe Browsing API due to invalid response." };
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout while checking URLs against Google Safe Browsing API.");
            return new SafeBrowsingResult { IsSafe = true, Description = "Check timed out." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected Error while checking URLs against Google Safe Browsing API.");
            return new SafeBrowsingResult { IsSafe = true, Description = "Failed to validate URL against Safe Browsing API." };
        }
    }
}
