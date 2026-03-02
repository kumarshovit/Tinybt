using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public class RedirectUrlHandler
    : IRequestHandler<RedirectUrlQuery, UrlMapping?>
{
  private readonly IUrlRepository _repo;

  public RedirectUrlHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<UrlMapping?> Handle(
      RedirectUrlQuery request,
      CancellationToken ct)
  {
    var url = await _repo.GetByShortCodeAsync(request.ShortCode);

    if (url == null || url.IsDeleted)
      return null;

    if (url.ExpirationDate.HasValue &&
        url.ExpirationDate < DateTime.UtcNow)
      return null;

    // ✅ Increment click count
    url.ClickCount++;

    // ✅ Create ClickLog (MATCHES YOUR MODEL EXACTLY)
    var clickLog = new ClickLog
    {
      ShortCode = url.ShortCode,
      VisitorId = Guid.NewGuid().ToString(),
      ClickedAt = DateTime.UtcNow,
      Browser = request.Browser ?? "Unknown",
      OS = request.OS ?? "Unknown",
      Country = request.Country ?? "Unknown",
      DeviceLanguage = request.DeviceLanguage ?? "Unknown",
      Referrer = request.Referrer ?? "Direct",
      DeviceType = request.DeviceType ?? "Unknown",
      IpAddress = request.IpAddress ?? "Unknown",
      RawHeaders = request.RawHeaders ?? "N/A"
    };

    await _repo.LogClickAsync(clickLog, ct);
    await _repo.UpdateAsync(url);

    return url;
  }
}
