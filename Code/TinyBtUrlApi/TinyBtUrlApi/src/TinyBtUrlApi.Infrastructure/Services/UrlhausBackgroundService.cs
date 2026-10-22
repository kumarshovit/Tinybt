using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Background hosted service that downloads and refreshes the URLhaus malware URL feed periodically.
/// On startup, it downloads the CSV feed and loads all URLs into the UrlhausFeedStore.
/// It then refreshes the feed at the configured interval (default: every 60 minutes).
/// </summary>
public class UrlhausBackgroundService : BackgroundService
{
    private readonly UrlhausFeedStore _feedStore;
    private readonly HttpClient _httpClient;
    private readonly UrlhausOptions _options;
    private readonly ILogger<UrlhausBackgroundService> _logger;

    public UrlhausBackgroundService(
        UrlhausFeedStore feedStore,
        IHttpClientFactory httpClientFactory,
        IOptions<UrlhausOptions> options,
        ILogger<UrlhausBackgroundService> logger)
    {
        _feedStore = feedStore;
        _httpClient = httpClientFactory.CreateClient("UrlhausFeed");
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("URLhaus feed is disabled via configuration. Skipping.");
            return;
        }

        _logger.LogInformation("URLhaus background service starting. Feed URL: {FeedUrl}, Refresh interval: {Interval} minutes",
            _options.FeedUrl, _options.RefreshIntervalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DownloadAndLoadFeedAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("URLhaus background service is stopping.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download/parse URLhaus feed. Will retry in {Interval} minutes.", _options.RefreshIntervalMinutes);
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_options.RefreshIntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task DownloadAndLoadFeedAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading URLhaus feed from {FeedUrl}...", _options.FeedUrl);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(_options.FeedDownloadTimeoutSeconds));

        using var request = new HttpRequestMessage(HttpMethod.Get, _options.FeedUrl);
        if (!string.IsNullOrWhiteSpace(_options.AuthKey))
        {
            request.Headers.TryAddWithoutValidation("Auth-Key", _options.AuthKey);
        }

        using var response = await _httpClient.SendAsync(request, cts.Token);
        response.EnsureSuccessStatusCode();

        var feedContent = await response.Content.ReadAsStringAsync(cts.Token);

        var newUrls = ParseFeed(feedContent);

        _feedStore.SwapUrls(newUrls);

        _logger.LogInformation("URLhaus feed loaded successfully: {Count} malware URLs", newUrls.Count);
    }

    /// <summary>
    /// Parses the URLhaus feed supporting both standard CSV and Adblock-format text feeds.
    /// Format 1 (Adblock mirror):
    ///   ||raw.githubusercontent.com/fnpsz/netflix-clone/...^$all
    /// Format 2 (Official CSV):
    ///   "id","dateadded","url","url_status","last_online","threat","tags","urlhaus_link","reporter"
    /// Format 3 (Plain URLs or host paths):
    ///   https://example.com/malware.exe
    /// </summary>
    private HashSet<string> ParseFeed(string content)
    {
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StringReader(content);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var trimmed = line.Trim();

            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#') || trimmed.StartsWith('!'))
                continue;

            try
            {
                // Format 1: Adblock rule (starts with ||)
                if (trimmed.StartsWith("||", StringComparison.Ordinal))
                {
                    var afterPipe = trimmed.Substring(2);
                    var caretIndex = afterPipe.IndexOf('^');
                    var target = caretIndex >= 0 ? afterPipe.Substring(0, caretIndex) : afterPipe;
                    target = target.Trim();

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        AddUrlVariants(urls, target);
                    }
                    continue;
                }

                // Format 2: CSV line (contains quoted columns)
                if (trimmed.StartsWith('"') && trimmed.Contains(','))
                {
                    var fields = ParseCsvLine(trimmed);
                    if (fields.Count >= 3)
                    {
                        var rawUrl = fields[2].Trim('"', ' ');
                        if (!string.IsNullOrWhiteSpace(rawUrl))
                        {
                            AddUrlVariants(urls, rawUrl);
                        }
                    }
                    continue;
                }

                // Format 3: Plain URL
                if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    AddUrlVariants(urls, trimmed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace("Skipping malformed URLhaus feed line: {Error}", ex.Message);
            }
        }

        return urls;
    }

    private static void AddUrlVariants(HashSet<string> set, string raw)
    {
        var clean = raw.Trim();
        if (string.IsNullOrWhiteSpace(clean)) return;

        set.Add(clean);

        if (clean.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var schemeless = clean.Substring(8);
            set.Add(schemeless);
            set.Add("http://" + schemeless);
        }
        else if (clean.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            var schemeless = clean.Substring(7);
            set.Add(schemeless);
            set.Add("https://" + schemeless);
        }
        else
        {
            set.Add("https://" + clean);
            set.Add("http://" + clean);
        }
    }

    /// <summary>
    /// Simple CSV line parser that handles quoted fields with commas inside them.
    /// </summary>
    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var currentField = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                // Check for escaped quote ""
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // Skip the next quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        fields.Add(currentField.ToString());
        return fields;
    }
}
