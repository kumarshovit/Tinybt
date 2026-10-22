using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

public class CloudflareUrlScannerService : ICloudflareUrlScannerService
{
    private readonly HttpClient _httpClient;
    private readonly CloudflareUrlScannerOptions _options;
    private readonly ILogger<CloudflareUrlScannerService> _logger;

    public CloudflareUrlScannerService(
        HttpClient httpClient,
        IOptions<CloudflareUrlScannerOptions> options,
        ILogger<CloudflareUrlScannerService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string?> SubmitScanAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiToken) || string.IsNullOrWhiteSpace(_options.AccountId))
        {
            _logger.LogWarning("Cloudflare URL Scanner is not enabled or credentials are missing.");
            return null;
        }

        try
        {
            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/scan";

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);
            request.Content = JsonContent.Create(new
            {
                url = url,
                visibility = "Unlisted"
            });

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Failed to submit scan to Cloudflare URL Scanner. Status: {Status}, Body: {Body}",
                    response.StatusCode, errorBody);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<CloudflareScanSubmitResponse>(cancellationToken: cancellationToken);
            return result?.Result?.Uuid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while submitting scan to Cloudflare for URL: {Url}", url);
            return null;
        }
    }

    public async Task<CloudflareScanResult> GetScanResultAsync(string scanUuid, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scanUuid))
            return CloudflareScanResult.Safe();

        try
        {
            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/scan/{scanUuid}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid };
            }

            var report = await response.Content.ReadFromJsonAsync<CloudflareScanReportResponse>(cancellationToken: cancellationToken);
            var verdicts = report?.Result?.Scan?.Verdicts?.Overall;

            if (verdicts != null)
            {
                if (verdicts.Malicious)
                {
                    var categories = verdicts.Categories ?? new List<string> { "Malicious" };
                    return CloudflareScanResult.Malicious(
                        scanUuid,
                        categories,
                        $"Cloudflare URL Scanner verdict: Malicious ({string.Join(", ", categories)})"
                    );
                }

                return CloudflareScanResult.Safe(scanUuid);
            }

            // Report not ready yet / still scanning
            return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid, IsPending = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while fetching Cloudflare scan result for UUID: {Uuid}", scanUuid);
            return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid, ErrorOccurred = true };
        }
    }

    public async Task<CloudflareScanResult> ScanAndPollAsync(string url, CancellationToken cancellationToken = default)
    {
        var scanUuid = await SubmitScanAsync(url, cancellationToken);
        if (string.IsNullOrWhiteSpace(scanUuid))
        {
            return CloudflareScanResult.Safe();
        }

        var timeout = TimeSpan.FromSeconds(_options.ScanTimeoutSeconds);
        var startTime = DateTime.UtcNow;

        // Poll every 3-5 seconds until completion or timeout
        while (DateTime.UtcNow - startTime < timeout && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(3000, cancellationToken);

            var result = await GetScanResultAsync(scanUuid, cancellationToken);
            if (result.IsMalicious)
            {
                _logger.LogWarning("Cloudflare detected malicious URL: {Url}. UUID: {Uuid}", url, scanUuid);
                return result;
            }

            if (!result.IsPending && !result.ErrorOccurred)
            {
                // Scan has completed safely
                return result;
            }
        }

        return CloudflareScanResult.Safe(scanUuid);
    }
}
