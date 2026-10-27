namespace TinyBtUrlApi.Core.Interfaces;

public interface IScreenshotQueue
{
    ValueTask EnqueueAsync(int urlMappingId, CancellationToken ct = default);
}
