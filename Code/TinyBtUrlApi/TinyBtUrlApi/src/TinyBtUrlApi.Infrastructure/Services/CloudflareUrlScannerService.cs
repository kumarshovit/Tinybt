using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
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

                // Handle HTTP 409 Conflict (scan already exists for this URL)
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    try
                    {
                        var conflictResult = JsonSerializer.Deserialize<CloudflareScanSubmitResponse>(errorBody);
                        if (!string.IsNullOrWhiteSpace(conflictResult?.Result?.Uuid))
                        {
                            _logger.LogInformation("Recovered existing scan UUID {Uuid} from 409 response for {Url}", conflictResult.Result.Uuid, url);
                            return conflictResult.Result.Uuid;
                        }
                    }
                    catch
                    {
                        // Ignore deserialization error and attempt regex recovery
                    }

                    var match = Regex.Match(
                        errorBody,
                        @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}");
                    if (match.Success)
                    {
                        _logger.LogInformation("Extracted existing scan UUID {Uuid} from 409 response for {Url}", match.Value, url);
                        return match.Value;
                    }

                    var searchRes = await SearchScanAsync(url, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(searchRes?.ScanUuid))
                    {
                        _logger.LogInformation("Found existing scan UUID {Uuid} via search for 409 Conflict URL {Url}", searchRes.ScanUuid, url);
                        return searchRes.ScanUuid;
                    }
                }

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
            var item = searchResult?.Results?.FirstOrDefault() ?? searchResult?.Result?.Tasks?.FirstOrDefault();
            var uuid = item?.Task?.Uuid ?? item?.Uuid ?? string.Empty;
            if (item?.Verdicts != null)
            {
                var isMalicious = item.Verdicts.Malicious == true || item.Verdicts.Overall?.Malicious == true;
                if (isMalicious)
                {
                    var categories = item.Verdicts.Categories ?? item.Verdicts.Overall?.Categories ?? new List<string> { "Malicious" };
                    return CloudflareScanResult.Malicious(
                        uuid,
                        categories,
                        $"Cloudflare URL Scanner verdict: Malicious ({string.Join(", ", categories)})"
                    );
                }

                return CloudflareScanResult.Safe(uuid);
            }

            if (!string.IsNullOrWhiteSpace(uuid))
            {
                return new CloudflareScanResult { IsSafe = true, ScanUuid = uuid, IsPending = true };
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

        // 1. Fast check: see if Cloudflare already has a scan verdict for this URL
        var existing = await SearchScanAsync(url, cancellationToken);
        if (existing != null)
        {
            if (existing.IsMalicious)
            {
                _logger.LogWarning("URL found to be malicious via Cloudflare scan intelligence: {Url}", url);
                return existing;
            }

            // If an existing scan has finished and is safe, return immediately without re-submitting
            if (!existing.IsPending)
            {
                return existing;
            }
        }

        // 2. Submit new scan (or reuse pending scan UUID if already queued)
        var scanUuid = existing?.ScanUuid ?? await SubmitScanAsync(url, cancellationToken);
        if (string.IsNullOrWhiteSpace(scanUuid))
        {
            _logger.LogWarning("Cloudflare scan submission was inconclusive or failed for URL {Url} due to an API issue. Returning error result so system falls back to other verifications.", url);
            return CloudflareScanResult.Inconclusive(null, "Cloudflare scan submission failed due to an API issue.");
        }

        // 3. Poll and wait for Cloudflare scanner while it is still running
        var timeout = TimeSpan.FromSeconds(Math.Max(1, _options.QuickCheckTimeoutSeconds));
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken);

            var result = await GetScanResultAsync(scanUuid, cancellationToken);
            if (result.IsMalicious)
            {
                _logger.LogWarning("Cloudflare quick check detected malicious URL: {Url}. UUID: {Uuid}", url, scanUuid);
                return result;
            }

            // If an API error occurred during polling, return immediately so other verifications can proceed
            if (result.ErrorOccurred)
            {
                _logger.LogWarning("Cloudflare scan result endpoint returned an API error for URL {Url}. Falling back to other verifications.", url);
                return result;
            }

            // If scan has completed and is no longer pending, return result
            if (!result.IsPending)
            {
                return result;
            }
        }

        // Scan remained pending after the extended timeout
        _logger.LogWarning("Cloudflare quick check for URL {Url} remained pending after {Timeout}s. UUID: {Uuid}.",
            url, timeout.TotalSeconds, scanUuid);
        return CloudflareScanResult.Pending(scanUuid, "Cloudflare scan was still running after timeout.");
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

    public async Task<byte[]?> DownloadScreenshotAsync(string scanUuid, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiToken) || string.IsNullOrWhiteSpace(_options.AccountId))
        {
            _logger.LogWarning("Cloudflare URL Scanner is disabled or credentials missing for screenshot download.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(scanUuid))
            return null;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(15)); // Strict 15s timeout

            var requestUrl = $"https://api.cloudflare.com/client/v4/accounts/{_options.AccountId}/urlscanner/v2/screenshots/{scanUuid}.png?resolution=desktop";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));

            var response = await _httpClient.SendAsync(request, cts.Token);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Screenshot for scan UUID {Uuid} is not ready yet (404 Not Found).", scanUuid);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogWarning("Failed to download screenshot for scan UUID {Uuid}. Status: {Status}, Body: {Body}",
                    scanUuid, response.StatusCode, errorBody);
                return null;
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (!string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(contentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Unexpected content type {ContentType} when downloading screenshot for scan UUID {Uuid}.",
                    contentType, scanUuid);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cts.Token);
            return bytes;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Screenshot download timed out after 15s for scan UUID {Uuid}.", scanUuid);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while downloading screenshot from Cloudflare for scan UUID {Uuid}.", scanUuid);
            return null;
        }
    }
}
