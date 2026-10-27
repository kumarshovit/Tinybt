using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ScreenshotBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ScreenshotQueue _queue;
    private readonly ILogger<ScreenshotBackgroundWorker> _logger;
    private DateTime _lastOrphanCleanup = DateTime.MinValue;

    public ScreenshotBackgroundWorker(
        IServiceScopeFactory scopeFactory,
        ScreenshotQueue queue,
        ILogger<ScreenshotBackgroundWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScreenshotBackgroundWorker started.");

        // Start dual execution:
        // 1. Channel consumer loop for immediate creation-time processing
        var channelTask = ProcessChannelQueueAsync(stoppingToken);

        // 2. Periodic 30-second sweep timer for database resilience + orphan cleanup
        var sweepTask = RunPeriodicSweepAsync(stoppingToken);

        await Task.WhenAll(channelTask, sweepTask);

        _logger.LogInformation("ScreenshotBackgroundWorker stopped.");
    }

    private async Task ProcessChannelQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var urlId in _queue.Reader.ReadAllAsync(stoppingToken))
                {
                    await ProcessUrlScreenshotAsync(urlId, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in screenshot queue channel reader loop.");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task RunPeriodicSweepAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        // Initial sweep on startup
        await SweepPendingScreenshotsAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await SweepPendingScreenshotsAsync(stoppingToken);
                await PerformPeriodicOrphanCleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in periodic screenshot sweep.");
            }
        }
    }

    private async Task SweepPendingScreenshotsAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUrlRepository>();

        var pendingUrls = await repo.GetPendingScreenshotsAsync(limit: 50, stoppingToken);
        if (pendingUrls.Count == 0)
            return;

        _logger.LogDebug("Periodic sweep found {Count} pending screenshot URLs.", pendingUrls.Count);

        foreach (var url in pendingUrls)
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            await ProcessUrlScreenshotAsync(url.Id, stoppingToken);
        }
    }

    private async Task ProcessUrlScreenshotAsync(int urlId, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IUrlRepository>();
            var cloudflare = scope.ServiceProvider.GetRequiredService<ICloudflareUrlScannerService>();
            var storage = scope.ServiceProvider.GetRequiredService<IScreenshotStorage>();

            var url = await repo.GetByIdAsync(urlId);
            if (url == null || url.IsDeleted || url.IsFlagged || url.IsPasswordProtected)
            {
                return;
            }

            if (url.ScreenshotStatus != ScreenshotStatus.Pending)
            {
                return;
            }

            // Exceeds max pending age of 5 minutes -> mark Failed
            if (DateTime.UtcNow - url.CreatedAt > TimeSpan.FromMinutes(5))
            {
                _logger.LogWarning("Screenshot processing for short code {ShortCode} exceeded 5-minute timeout. Marking Failed.", url.ShortCode);
                await repo.UpdateScreenshotStatusAsync(url.Id, ScreenshotStatus.Failed, null, url.CloudflareScanUuid, stoppingToken);
                return;
            }

            if (string.IsNullOrWhiteSpace(url.CloudflareScanUuid))
            {
                _logger.LogWarning("URL {ShortCode} has no CloudflareScanUuid. Marking ScreenshotStatus as None.", url.ShortCode);
                await repo.UpdateScreenshotStatusAsync(url.Id, ScreenshotStatus.None, null, null, stoppingToken);
                return;
            }

            // Attempt to download screenshot
            var imageBytes = await cloudflare.DownloadScreenshotAsync(url.CloudflareScanUuid, stoppingToken);
            if (imageBytes != null && imageBytes.Length > 0)
            {
                var relativePath = await storage.SaveAsync(url.CloudflareScanUuid, imageBytes, stoppingToken);
                await repo.UpdateScreenshotStatusAsync(url.Id, ScreenshotStatus.Ready, relativePath, url.CloudflareScanUuid, stoppingToken);
                _logger.LogInformation("Successfully saved screenshot for short code {ShortCode} at {Path}.", url.ShortCode, relativePath);
            }
            else
            {
                _logger.LogDebug("Screenshot for {ShortCode} (UUID: {Uuid}) not ready yet; will be retried in next sweep.",
                    url.ShortCode, url.CloudflareScanUuid);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process screenshot for URL ID {UrlId}.", urlId);
        }
    }

    private async Task PerformPeriodicOrphanCleanupAsync(CancellationToken stoppingToken)
    {
        if (DateTime.UtcNow - _lastOrphanCleanup < TimeSpan.FromHours(1))
            return;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<IScreenshotStorage>();

            _logger.LogInformation("Running hourly orphan screenshot cleanup (files >24h old)...");
            var deletedCount = await storage.CleanupOrphanFilesAsync(TimeSpan.FromHours(24), stoppingToken);
            _lastOrphanCleanup = DateTime.UtcNow;

            if (deletedCount > 0)
            {
                _logger.LogInformation("Orphan screenshot cleanup completed. Deleted {Count} file(s).", deletedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing hourly orphan screenshot cleanup.");
        }
    }
}
