using Microsoft.Extensions.Logging;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Thread-safe singleton store that holds the URLhaus malware URL database in memory.
/// The BackgroundService downloads and refreshes this data periodically.
/// </summary>
public class UrlhausFeedStore
{
    private volatile HashSet<string> _malwareUrls = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _swapLock = new();
    private readonly ILogger<UrlhausFeedStore> _logger;

    public UrlhausFeedStore(ILogger<UrlhausFeedStore> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Whether the feed has been loaded at least once.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// The number of malware URLs currently in the database.
    /// </summary>
    public int Count => _malwareUrls.Count;

    /// <summary>
    /// Atomically replaces the current URL set with a new one.
    /// </summary>
    public void SwapUrls(HashSet<string> newUrls)
    {
        lock (_swapLock)
        {
            _malwareUrls = newUrls;
            IsLoaded = true;
        }
        _logger.LogInformation("URLhaus feed store updated: {Count} malware URLs loaded", newUrls.Count);
    }

    /// <summary>
    /// Checks if a URL exists in the malware database.
    /// Performs normalized lookups (with/without trailing slash, with/without scheme).
    /// </summary>
    public bool Contains(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        var currentSet = _malwareUrls;
        var trimmed = url.Trim();

        // 1. Direct check with/without trailing slash
        if (CheckVariations(currentSet, trimmed)) return true;

        // 2. Check without scheme (e.g. raw.githubusercontent.com/...)
        var schemeless = StripScheme(trimmed);
        if (!string.Equals(schemeless, trimmed, StringComparison.OrdinalIgnoreCase) &&
            CheckVariations(currentSet, schemeless))
        {
            return true;
        }

        // 3. If input didn't have scheme, check with https:// and http://
        if (!trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            if (CheckVariations(currentSet, "https://" + trimmed)) return true;
            if (CheckVariations(currentSet, "http://" + trimmed)) return true;
        }

        return false;
    }

    private static bool CheckVariations(HashSet<string> set, string target)
    {
        if (set.Contains(target)) return true;

        var trimmedSlash = target.TrimEnd('/');
        if (set.Contains(trimmedSlash)) return true;

        if (set.Contains(trimmedSlash + "/")) return true;

        return false;
    }

    private static string StripScheme(string url)
    {
        if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return url.Substring(8);
        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            return url.Substring(7);
        return url;
    }
}
