namespace TinyBtUrlApi.Core.Interfaces;

public interface IScreenshotStorage
{
    Task<string> SaveAsync(string scanUuid, byte[] imageBytes, CancellationToken ct = default);
    Task<bool> ExistsAsync(string relativePath, CancellationToken ct = default);
    Task<bool> DeleteAsync(string relativePath, CancellationToken ct = default);
    Task<bool> IsPathReferencedByAnotherLiveLinkAsync(string relativePath, int excludeUrlId, CancellationToken ct = default);
    Task<int> CleanupOrphanFilesAsync(TimeSpan olderThan, CancellationToken ct = default);
}
