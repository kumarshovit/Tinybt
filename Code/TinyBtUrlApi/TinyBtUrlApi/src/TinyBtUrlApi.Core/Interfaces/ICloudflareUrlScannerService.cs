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

    /// <summary>
    /// Searches existing Cloudflare URL scan intelligence for a given URL or domain.
    /// Returns cached scan verdict if found, or null if no previous scan exists.
    /// </summary>
    Task<CloudflareScanResult?> SearchScanAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fast check for URL creation or real-time evaluation: checks search cache first,
    /// submits scan, and polls up to a short fast-timeout (default 5s) before failing open.
    /// </summary>
    Task<CloudflareScanResult> QuickCheckUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the destination screenshot (PNG) for a completed scan UUID.
    /// Returns binary image bytes or null if still pending or failed.
    /// </summary>
    Task<byte[]?> DownloadScreenshotAsync(string scanUuid, CancellationToken cancellationToken = default);
}
