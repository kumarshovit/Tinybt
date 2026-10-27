using System.Threading.Channels;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ScreenshotQueue : IScreenshotQueue
{
    private readonly Channel<int> _channel;

    public ScreenshotQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        };
        _channel = Channel.CreateUnbounded<int>(options);
    }

    public ValueTask EnqueueAsync(int urlMappingId, CancellationToken ct = default)
    {
        return _channel.Writer.WriteAsync(urlMappingId, ct);
    }

    public ChannelReader<int> Reader => _channel.Reader;
}
