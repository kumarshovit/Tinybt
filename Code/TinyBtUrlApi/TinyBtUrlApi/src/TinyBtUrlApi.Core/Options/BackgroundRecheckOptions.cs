namespace TinyBtUrlApi.Core.Options;

public class BackgroundRecheckOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval in hours between scanning runs.
    /// Default: 6 hours.
    /// </summary>
    public int IntervalHours { get; set; } = 6;

    /// <summary>
    /// Maximum number of active URLs to check in each run.
    /// Default: 100.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Whether to include Cloudflare URL Scanner in the periodic recheck.
    /// </summary>
    public bool IncludeCloudflareScanner { get; set; } = false;
}
