using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public class ContinueRedirectHandler : IRequestHandler<ContinueRedirectCommand, ContinueRedirectResult>
{
    private readonly IUrlRepository _repo;
    private readonly IUrlAccessTokenService _tokenService;

    public ContinueRedirectHandler(
        IUrlRepository repo,
        IUrlAccessTokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async ValueTask<ContinueRedirectResult> Handle(
        ContinueRedirectCommand request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ShortCode) || string.IsNullOrWhiteSpace(request.Token))
        {
            return ContinueRedirectResult.Invalid("Short code and token are required.");
        }

        // 1. Authoritative validation of server-signed token
        var payload = _tokenService.TryUnprotect(request.Token, request.ShortCode);
        if (payload is null)
        {
            return ContinueRedirectResult.Invalid("Invalid or expired token. Please visit the link again.");
        }

        // 2. Load URL from DB
        var url = await _repo.GetByShortCodeAsync(request.ShortCode);
        if (url is null || url.IsDeleted)
        {
            return ContinueRedirectResult.NotFound("The requested URL was not found.");
        }

        // 3. Expiration check
        if (url.ExpirationDate.HasValue && url.ExpirationDate.Value < DateTime.UtcNow)
        {
            return ContinueRedirectResult.Expired("This link has expired.");
        }

        // 4. Safety flag check (authoritative from DB)
        if (url.IsFlagged)
        {
            return ContinueRedirectResult.Blocked(url.FlaggedReason ?? "This link has been blocked as unsafe.");
        }

        // 5. 🎯 Increment click count ONLY here upon user clicking Continue
        url.ClickCount++;

        // 6. Device, OS, Browser, and Traffic Source analysis
        var userAgent = request.UserAgent ?? string.Empty;

        var deviceType =
            userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase)
                ? "Tablet"
                : userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase) ||
                  userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
                  userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)
                    ? "Mobile"
                    : request.DeviceType ?? "Desktop";

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

        var referrer = request.Referrer ?? "";
        var trafficSource = !string.IsNullOrWhiteSpace(payload.Source) && payload.Source != "Direct"
            ? payload.Source
            : (
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

        var country =
            request.IpAddress == "::1" || request.IpAddress == "127.0.0.1"
                ? "India"
                : request.Country ?? "Unknown";

        var visitorId = $"{request.IpAddress}_{browser}_{deviceType}_{request.DeviceLanguage}";

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
            UserAgent = request.UserAgent ?? "Unknown",
            RawHeaders = request.RawHeaders ?? "N/A"
        };

        await _repo.LogClickAsync(clickLog, ct);
        await _repo.UpdateAsync(url);

        return ContinueRedirectResult.Ok(url.LongUrl);
    }
}
