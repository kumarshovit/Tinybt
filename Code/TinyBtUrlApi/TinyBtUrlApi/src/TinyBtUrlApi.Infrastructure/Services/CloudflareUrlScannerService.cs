using System.Net;
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
            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/v2/scan";

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
            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/v2/result/{scanUuid}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            // Cloudflare URL Scanner returns HTTP 404 while scan is still queued / processing
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid, IsPending = true };
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Cloudflare result endpoint returned non-success code {StatusCode}: {Body}",
                    response.StatusCode, errorBody);
                return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid, ErrorOccurred = true };
            }

            var report = await response.Content.ReadFromJsonAsync<CloudflareScanReportResponse>(cancellationToken: cancellationToken);

            if (report?.Result != null)
            {
                // Check if task is still running or queued
                var taskStatus = report.Result.Task?.Status;
                if (string.Equals(taskStatus, "queued", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(taskStatus, "running", StringComparison.OrdinalIgnoreCase))
                {
                    return new CloudflareScanResult { IsSafe = true, ScanUuid = scanUuid, IsPending = true };
                }

                if (report.Result.IsMalicious())
                {
                    var categories = report.Result.GetCategories();
                    if (!categories.Any()) categories.Add("Malicious");

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

    public async Task<CloudflareScanResult?> SearchScanAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiToken) || string.IsNullOrWhiteSpace(_options.AccountId))
            return null;

        try
        {
            var encodedUrl = Uri.EscapeDataString(url);
            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/v2/search?q=page.url:\"{encodedUrl}\"&size=1";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var searchResult = await response.Content.ReadFromJsonAsync<CloudflareScanSearchResponse>(cancellationToken: cancellationToken);
            var item = searchResult?.Result?.Tasks?.FirstOrDefault();
            if (item?.Verdicts != null)
            {
                var isMalicious = item.Verdicts.Malicious == true || item.Verdicts.Overall?.Malicious == true;
                if (isMalicious)
                {
                    var categories = item.Verdicts.Categories ?? item.Verdicts.Overall?.Categories ?? new List<string> { "Malicious" };
                    return CloudflareScanResult.Malicious(
                        item.Uuid ?? string.Empty,
                        categories,
                        $"Cloudflare URL Scanner historical verdict: Malicious ({string.Join(", ", categories)})"
                    );
                }

                return CloudflareScanResult.Safe(item.Uuid);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error checking Cloudflare search intelligence for URL: {Url}", url);
            return null;
        }
    }

    public async Task<CloudflareScanResult> QuickCheckUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiToken) || string.IsNullOrWhiteSpace(_options.AccountId))
            return CloudflareScanResult.Safe();

        // 1. Fast check: see if Cloudflare already has an active scan verdict for this URL
        var existing = await SearchScanAsync(url, cancellationToken);
        if (existing != null && existing.IsMalicious)
        {
            _logger.LogWarning("URL found to be malicious via Cloudflare scan intelligence: {Url}", url);
            return existing;
        }

        // 2. Submit new scan
        var scanUuid = await SubmitScanAsync(url, cancellationToken);
        if (string.IsNullOrWhiteSpace(scanUuid))
            return CloudflareScanResult.Safe();

        // 3. Fast poll with a short timeout (default 5s)
        var timeout = TimeSpan.FromSeconds(Math.Max(1, _options.QuickCheckTimeoutSeconds));
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1500, cancellationToken);

            var result = await GetScanResultAsync(scanUuid, cancellationToken);
            if (result.IsMalicious)
            {
                _logger.LogWarning("Cloudflare quick check detected malicious URL: {Url}. UUID: {Uuid}", url, scanUuid);
                return result;
            }

            if (!result.IsPending && !result.ErrorOccurred)
            {
                return result;
            }
        }

        // Fail open if quick check times out (background recheck or redirect recheck will follow up)
        return CloudflareScanResult.Safe(scanUuid);
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

        // Poll every 3 seconds until completion or timeout
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
