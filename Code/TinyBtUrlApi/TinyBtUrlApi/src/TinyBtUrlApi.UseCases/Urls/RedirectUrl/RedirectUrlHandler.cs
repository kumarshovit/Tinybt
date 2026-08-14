using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public class RedirectUrlHandler(IUrlRepository repo)
    : IRequestHandler<RedirectUrlQuery, RedirectResult>
{
  public async ValueTask<RedirectResult> Handle(
      RedirectUrlQuery request,
      CancellationToken ct)
  {
    var url = await repo.GetByShortCodeAsync(request.ShortCode);

    if (url is null || url.IsDeleted)
      return new RedirectResult(RedirectStatus.NotFound);

    if (url.ExpirationDate.HasValue &&
        url.ExpirationDate.Value < DateTime.UtcNow)
      return new RedirectResult(RedirectStatus.Expired);

    // 🔐 Password-protected — early exit, no click logged
    if (url.IsPasswordProtected)
      return new RedirectResult(RedirectStatus.PasswordProtected, url);

    url.ClickCount++;

    var userAgent = request.Browser ?? string.Empty;

    // =========================
    // DEVICE TYPE DETECTION
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
    // OS DETECTION
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
    // BROWSER DETECTION
    // =========================
    var browser =
        userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)
            ? "Edge"
            : userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase) &&
              !userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase)
                ? "Chrome"
                : userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase)
                    ? "Firefox"
                    : userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase)
                        ? "Safari"
                        : "Unknown";

    // =========================
    // TRAFFIC SOURCE DETECTION
    // =========================
    var referrer = request.Referrer ?? "";

    var trafficSource = request.SourceOverride ?? (
        string.IsNullOrWhiteSpace(referrer) ? "Direct" :
        referrer.Contains("google", StringComparison.OrdinalIgnoreCase) ||
        referrer.Contains("bing", StringComparison.OrdinalIgnoreCase) ||
        referrer.Contains("yahoo", StringComparison.OrdinalIgnoreCase)
            ? "Search"
            : referrer.Contains("facebook", StringComparison.OrdinalIgnoreCase) ||
              referrer.Contains("twitter", StringComparison.OrdinalIgnoreCase) ||
              referrer.Contains("linkedin", StringComparison.OrdinalIgnoreCase)
                ? "Social"
                : "Referral"
    );

    // =========================
    // COUNTRY FIX FOR LOCALHOST
    // =========================
    var country =
        request.IpAddress == "::1" || request.IpAddress == "127.0.0.1"
            ? "India"
            : request.Country ?? "Unknown";

    var visitorId = $"{request.IpAddress}_{browser}_{deviceType}_{request.DeviceLanguage}";
    // =========================
    // CREATE CLICK LOG
    // =========================
    var clickLog = new ClickLog
    {
      ShortCode = url.ShortCode,
      VisitorId = visitorId,
      ClickedAt = DateTime.UtcNow,
      Browser = browser,
      OS = os,
      Country = country,
      DeviceLanguage = request.DeviceLanguage ?? "Unknown",
      Referrer = referrer,
      Source = trafficSource,
      DeviceType = deviceType,
      IpAddress = request.IpAddress ?? "Unknown",
      UserAgent = request.Browser ?? "Unknown",
      RawHeaders = request.RawHeaders ?? "N/A"
    };

    await repo.LogClickAsync(clickLog, ct);
    await repo.UpdateAsync(url);

    return new RedirectResult(RedirectStatus.Found, url);
  }
}
