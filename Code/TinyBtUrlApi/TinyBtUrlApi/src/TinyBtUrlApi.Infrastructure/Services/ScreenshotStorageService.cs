using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ScreenshotStorageService : IScreenshotStorage
{
    private static readonly byte[] PngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

    private readonly AppDbContext _dbContext;
    private readonly ScreenshotOptions _options;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ScreenshotStorageService> _logger;
    private readonly string _absoluteStorageDirectory;

    public ScreenshotStorageService(
        AppDbContext dbContext,
        IOptions<ScreenshotOptions> options,
        IWebHostEnvironment env,
        ILogger<ScreenshotStorageService> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _env = env;
        _logger = logger;

        var configuredPath = string.IsNullOrWhiteSpace(_options.StoragePath)
            ? "wwwroot/screenshots"
            : _options.StoragePath;

        _absoluteStorageDirectory = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(_env.ContentRootPath, configuredPath);

        if (!Directory.Exists(_absoluteStorageDirectory))
        {
            Directory.CreateDirectory(_absoluteStorageDirectory);
        }
    }

    public async Task<string> SaveAsync(string scanUuid, byte[] imageBytes, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(scanUuid))
            throw new ArgumentException("Scan UUID cannot be empty.", nameof(scanUuid));

        if (imageBytes == null || imageBytes.Length == 0)
            throw new ArgumentException("Image bytes cannot be empty.", nameof(imageBytes));

        // 1. Max Size Check (5 MB)
        if (imageBytes.Length > _options.MaxSizeBytes)
        {
            throw new InvalidOperationException($"Image size ({imageBytes.Length} bytes) exceeds maximum permitted limit ({_options.MaxSizeBytes} bytes).");
        }

        // 2. PNG Magic Bytes Verification
        if (!IsValidPng(imageBytes))
        {
            throw new InvalidOperationException("Downloaded file failed PNG magic byte validation.");
        }

        var fileName = $"{scanUuid}.png";
        var finalFilePath = Path.Combine(_absoluteStorageDirectory, fileName);
        var tempFilePath = Path.Combine(_absoluteStorageDirectory, $"{fileName}.tmp.{Guid.NewGuid():N}");

        try
        {
            // 3. Write to temporary file
            await File.WriteAllBytesAsync(tempFilePath, imageBytes, ct);

            // 4. Atomic move to target destination
            File.Move(tempFilePath, finalFilePath, overwrite: true);
        }
        catch
        {
            if (File.Exists(tempFilePath))
            {
                try { File.Delete(tempFilePath); } catch { /* best effort */ }
            }
            throw;
        }

        // Return relative path standard for serving / storing
        return $"screenshots/{fileName}";
    }

    public Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.FromResult(false);

        var fileName = Path.GetFileName(relativePath);
        var absolutePath = Path.Combine(_absoluteStorageDirectory, fileName);
        return Task.FromResult(File.Exists(absolutePath));
    }

    public Task<bool> DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.FromResult(false);

        try
        {
            var fileName = Path.GetFileName(relativePath);
            var absolutePath = Path.Combine(_absoluteStorageDirectory, fileName);

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
                _logger.LogInformation("Deleted screenshot file: {Path}", absolutePath);
                return Task.FromResult(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete screenshot file: {Path}", relativePath);
        }

        return Task.FromResult(false);
    }

    public async Task<bool> IsPathReferencedByAnotherLiveLinkAsync(string relativePath, int excludeUrlId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        var normalizedPath = relativePath.Replace('\\', '/');

        return await _dbContext.UrlMappings
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                        && !x.IsFlagged
                        && x.Id != excludeUrlId
                        && (x.ScreenshotPath == normalizedPath || x.ScreenshotPath == relativePath),
                ct);
    }

    public async Task<int> CleanupOrphanFilesAsync(TimeSpan olderThan, CancellationToken ct = default)
    {
        try
        {
            if (!Directory.Exists(_absoluteStorageDirectory))
                return 0;

            // Single database query to gather all live referenced screenshot paths
            var liveReferencedPaths = (await _dbContext.UrlMappings
                .AsNoTracking()
                .Where(x => !x.IsDeleted && !x.IsFlagged && x.ScreenshotPath != null)
                .Select(x => x.ScreenshotPath!)
                .Distinct()
                .ToListAsync(ct))
                .Select(Path.GetFileName)
                .Where(f => !string.IsNullOrEmpty(f))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var cutoff = DateTime.UtcNow.Subtract(olderThan);
            var files = Directory.GetFiles(_absoluteStorageDirectory, "*.png");
            int deletedCount = 0;

            foreach (var file in files)
            {
                if (ct.IsCancellationRequested)
                    break;

                var fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTimeUtc < cutoff)
                {
                    if (!liveReferencedPaths.Contains(fileInfo.Name))
                    {
                        try
                        {
                            fileInfo.Delete();
                            deletedCount++;
                            _logger.LogInformation("Cleaned up orphaned screenshot file: {File}", fileInfo.Name);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Could not delete orphan file {File}", fileInfo.Name);
                        }
                    }
                }
            }

            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while performing orphan screenshot cleanup.");
            return 0;
        }
    }

    private static bool IsValidPng(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 8) return false;
        return bytes.Slice(0, 8).SequenceEqual(PngHeader);
    }
}
