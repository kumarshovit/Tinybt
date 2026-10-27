using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

/// <summary>
/// Periodic background worker that scans stored active URLs against threat intelligence feeds.
/// </summary>
public class UrlSafetyRecheckBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<BackgroundRecheckOptions> _options;
    private readonly ILogger<UrlSafetyRecheckBackgroundService> _logger;

    public UrlSafetyRecheckBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<BackgroundRecheckOptions> options,
        ILogger<UrlSafetyRecheckBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = _options.Value;
        if (!config.Enabled)
        {
            _logger.LogInformation("URL Safety Recheck Background Service is disabled.");
            return;
        }

        _logger.LogInformation("URL Safety Recheck Background Service started. Interval: {Hours} hours, BatchSize: {BatchSize}",
            config.IntervalHours, config.BatchSize);

        // Initial delay to avoid slowing down startup
        try
        {
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunSafetyRecheckAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during URL safety recheck execution.");
            }

            try
            {
                var interval = TimeSpan.FromHours(Math.Max(1, config.IntervalHours));
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("URL Safety Recheck Background Service stopped.");
    }

    private async Task RunSafetyRecheckAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUrlRepository>();
        var safetyOrchestrator = scope.ServiceProvider.GetRequiredService<IUrlSafetyOrchestrator>();
        var config = _options.Value;

        var urls = await repository.GetUrlsForSafetyRecheckAsync(config.BatchSize, cancellationToken);
        if (urls.Count == 0)
        {
            _logger.LogDebug("No URLs pending safety recheck.");
            return;
        }

        _logger.LogInformation("Starting safety recheck batch for {Count} URLs.", urls.Count);

        int checkedCount = 0;
        int flaggedCount = 0;

        foreach (var url in urls)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var result = await safetyOrchestrator.QuickCheckAsync(url.LongUrl, cancellationToken);
                checkedCount++;

                if (!result.IsSafe)
                {
                    flaggedCount++;
                    _logger.LogWarning("Background safety recheck flagged URL {ShortCode} ({Url}) as unsafe: {ThreatSource} - {ThreatType}",
                        url.ShortCode, url.LongUrl, result.ThreatSource, result.ThreatType);

                    await repository.FlagUrlAsync(url.Id, $"{result.ThreatSource}:{result.ThreatType}", cancellationToken);

                    // Clean up screenshot file if not referenced by any other live link
                    if (!string.IsNullOrWhiteSpace(url.ScreenshotPath))
                    {
                        var storage = scope.ServiceProvider.GetService<IScreenshotStorage>();
                        if (storage != null)
                        {
                            var isReferenced = await storage.IsPathReferencedByAnotherLiveLinkAsync(url.ScreenshotPath, url.Id, cancellationToken);
                            if (!isReferenced)
                            {
                                await storage.DeleteAsync(url.ScreenshotPath, cancellationToken);
                            }
                        }
                    }
                }
                else if (config.IncludeCloudflareScanner)
                {
                    var cloudflareScanner = scope.ServiceProvider.GetService<ICloudflareUrlScannerService>();
                    if (cloudflareScanner != null)
                    {
                        var cfOptions = scope.ServiceProvider.GetService<IOptions<CloudflareUrlScannerOptions>>()?.Value;
                        var delayMs = cfOptions?.DelayBetweenScansMs ?? 3000;
                        if (delayMs > 0)
                        {
                            await Task.Delay(delayMs, cancellationToken);
                        }

                        var cfResult = await cloudflareScanner.ScanAndPollAsync(url.LongUrl, cancellationToken);
                        if (cfResult.IsMalicious)
                        {
                            flaggedCount++;
                            _logger.LogWarning("Cloudflare URL Scanner flagged URL {ShortCode} ({Url}) as malicious: {Categories}",
                                url.ShortCode, url.LongUrl, string.Join(", ", cfResult.Categories));

                            await repository.FlagUrlAsync(url.Id, $"cloudflare:malicious_{string.Join("_", cfResult.Categories)}", cancellationToken);

                            if (!string.IsNullOrWhiteSpace(url.ScreenshotPath))
                            {
                                var storage = scope.ServiceProvider.GetService<IScreenshotStorage>();
                                if (storage != null)
                                {
                                    var isReferenced = await storage.IsPathReferencedByAnotherLiveLinkAsync(url.ScreenshotPath, url.Id, cancellationToken);
                                    if (!isReferenced)
                                    {
                                        await storage.DeleteAsync(url.ScreenshotPath, cancellationToken);
                                    }
                                }
                            }
                            continue;
                        }
                    }

                    await repository.UpdateLastSafetyCheckAsync(url.Id, cancellationToken);
                }
                else
                {
                    await repository.UpdateLastSafetyCheckAsync(url.Id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to recheck URL ID {UrlId} ({Url}).", url.Id, url.LongUrl);
            }
        }

        _logger.LogInformation("Completed safety recheck batch: {Checked} checked, {Flagged} flagged as unsafe.",
            checkedCount, flaggedCount);
    }
}
