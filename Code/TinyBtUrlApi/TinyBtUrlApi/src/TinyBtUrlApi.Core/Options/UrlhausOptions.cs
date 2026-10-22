namespace TinyBtUrlApi.Core.Options;

/// <summary>
/// Configuration options for the URLhaus (abuse.ch) malware URL feed.
/// </summary>
public class UrlhausOptions
{
    /// <summary>
    /// URL to download the URLhaus malware feed.
    /// Default: https://malware-filter.gitlab.io/urlhaus-filter/urlhaus-filter-online.txt
    /// Fallback / official abuse.ch: https://urlhaus.abuse.ch/downloads/csv_online/
    /// </summary>
    public string FeedUrl { get; set; } = "https://malware-filter.gitlab.io/urlhaus-filter/urlhaus-filter-online.txt";

    /// <summary>
    /// Optional API key / Auth-Key if using abuse.ch authenticated endpoints.
    /// </summary>
    public string? AuthKey { get; set; }

    /// <summary>
    /// How often to refresh the feed (in minutes). Default: 60 minutes.
    /// </summary>
    public int RefreshIntervalMinutes { get; set; } = 60;

    /// <summary>
    /// Enable or disable URLhaus checking. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Timeout for downloading the feed in seconds. Default: 60.
    /// </summary>
    public int FeedDownloadTimeoutSeconds { get; set; } = 60;
}
