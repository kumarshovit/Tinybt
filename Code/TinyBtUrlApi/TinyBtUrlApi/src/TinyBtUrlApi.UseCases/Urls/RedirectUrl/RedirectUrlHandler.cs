using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public class RedirectUrlHandler(IUrlRepository repo)
    : IRequestHandler<RedirectUrlQuery, UrlMapping?>
{
  public async ValueTask<UrlMapping?> Handle(
      RedirectUrlQuery request,
      CancellationToken ct)
  {
    var url = await repo.GetByShortCodeAsync(request.ShortCode);

    if (url is null || url.IsDeleted)
      return null;

    if (url.ExpirationDate.HasValue &&
        url.ExpirationDate.Value < DateTime.UtcNow)
      return null;

    // ✅ Increment Click Count
    url.ClickCount++;

    var userAgent = request.Browser ?? string.Empty;

    // =========================
    // ✅ DEVICE TYPE DETECTION
    // =========================
    var deviceType =
        userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) ||
        userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)
            ? "Tablet"
            : userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
              userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                ? "Mobile"
                : "Desktop";

    // =========================
    // ✅ OS DETECTION
    // =========================
    var os =
        userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase)
            ? "Windows"
            : userAgent.Contains("Mac", StringComparison.OrdinalIgnoreCase)
                ? "MacOS"
                : userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase)
                    ? "Android"
                    : userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                      userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)
                        ? "iOS"
                        : request.OS ?? "Unknown";

    // =========================
    // ✅ BROWSER DETECTION
    // =========================
    var browser =
        userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)
            ? "Edge"
            : userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase)
                ? "Chrome"
                : userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)
                    ? "Firefox"
                    : userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)
                        ? "Safari"
                        : request.Browser ?? "Unknown";

    // =========================
    // ✅ CREATE CLICK LOG
    // =========================
    var clickLog = new ClickLog
    {
      ShortCode = url.ShortCode,
      VisitorId = Guid.NewGuid().ToString(),
      ClickedAt = DateTime.UtcNow,
      Browser = browser,
      OS = os,
      Country = request.Country ?? "Unknown",
      DeviceLanguage = request.DeviceLanguage ?? "Unknown",
      Referrer = request.Referrer ?? "Direct",
      DeviceType = deviceType,
      IpAddress = request.IpAddress ?? "Unknown",
      RawHeaders = request.RawHeaders ?? "N/A"
    };

    await repo.LogClickAsync(clickLog, ct);
    await repo.UpdateAsync(url);

    return url;
  }
}
