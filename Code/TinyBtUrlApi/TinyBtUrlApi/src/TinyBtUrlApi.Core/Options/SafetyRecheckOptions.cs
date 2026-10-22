namespace TinyBtUrlApi.Core.Options;

public class SafetyRecheckOptions
{
    /// <summary>
    /// Time in seconds to cache the safety check result.
    /// If a URL was checked within this window, skip recheck at redirect time.
    /// Default: 300 seconds (5 minutes).
    /// </summary>
    public int CacheWindowSeconds { get; set; } = 300;

    /// <summary>
    /// Enable/disable redirect-time safety rechecks.
    /// </summary>
    public bool EnableRedirectTimeRecheck { get; set; } = true;
}
