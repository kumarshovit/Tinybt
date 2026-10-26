namespace TinyBtUrlApi.Core.Options;

public class CloudflareUrlScannerOptions
{
    public bool Enabled { get; set; } = true;
    public string ApiToken { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// Maximum time in seconds to wait for a submitted scan to complete.
    /// Default: 90 seconds.
    /// </summary>
    public int ScanTimeoutSeconds { get; set; } = 90;

    /// <summary>
    /// Maximum time in seconds to wait for a quick scan check during URL creation/orchestration.
    /// Default: 60 seconds.
    /// </summary>
    public int QuickCheckTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Delay between successive scans in milliseconds (to respect rate limits).
    /// Default: 5000 ms.
    /// </summary>
    public int DelayBetweenScansMs { get; set; } = 5000;
}
