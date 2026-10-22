using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Service for performing deep behavioral and network URL scans using Cloudflare URL Scanner API.
/// </summary>
public interface ICloudflareUrlScannerService
{
    /// <summary>
    /// Submits a URL to Cloudflare for asynchronous behavioral analysis.
    /// Returns the scan UUID if successfully submitted.
    /// </summary>
    Task<string?> SubmitScanAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Polls Cloudflare for the result of a submitted scan.
    /// </summary>
    Task<CloudflareScanResult> GetScanResultAsync(string scanUuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits and polls until scan completion or timeout. Recommended for background workers.
    /// </summary>
    Task<CloudflareScanResult> ScanAndPollAsync(string url, CancellationToken cancellationToken = default);
}
