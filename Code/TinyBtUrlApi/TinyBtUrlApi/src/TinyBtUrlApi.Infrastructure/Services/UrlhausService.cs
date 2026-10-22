using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Service implementation that checks URLs against the in-memory URLhaus malware database.
/// Lookups are O(1) HashSet operations with ~0ms latency.
/// </summary>
public class UrlhausService : IUrlhausService
{
    private readonly UrlhausFeedStore _feedStore;
    private readonly UrlhausOptions _options;
    private readonly ILogger<UrlhausService> _logger;

    public UrlhausService(
        UrlhausFeedStore feedStore,
        IOptions<UrlhausOptions> options,
        ILogger<UrlhausService> logger)
    {
        _feedStore = feedStore;
        _options = options.Value;
        _logger = logger;
    }

    public Task<UrlhausResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        return CheckUrlsAsync(new[] { url }, cancellationToken);
    }

    public Task<UrlhausResult> CheckUrlsAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        // If URLhaus is disabled, always return safe
        if (!_options.Enabled)
        {
            return Task.FromResult(new UrlhausResult { IsSafe = true, Description = "URLhaus check is disabled." });
        }

        // If feed hasn't loaded yet, fail open (allow the URL through)
        // This prevents blocking all URLs if the feed download fails on startup
        if (!_feedStore.IsLoaded)
        {
            _logger.LogWarning("URLhaus feed has not been loaded yet. Allowing URL through (fail-open).");
            return Task.FromResult(new UrlhausResult { IsSafe = true, Description = "URLhaus feed not yet loaded; skipped check." });
        }

        var urlList = urls.Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();
        if (!urlList.Any())
        {
            return Task.FromResult(new UrlhausResult { IsSafe = true });
        }

        foreach (var url in urlList)
        {
            if (_feedStore.Contains(url))
            {
                _logger.LogWarning("URL blocked by URLhaus malware database. URL: {Url}", url);

                return Task.FromResult(new UrlhausResult
                {
                    IsSafe = false,
                    ThreatType = "malware_download",
                    FlaggedUrl = url,
                    Description = $"The URL '{url}' has been identified as a malware distribution URL by URLhaus (abuse.ch)."
                });
            }
        }

        return Task.FromResult(new UrlhausResult { IsSafe = true });
    }
}
