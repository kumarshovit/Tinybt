using BCrypt.Net;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.VerifyUrlPassword;

/// <summary>
/// Handles password verification for a protected short URL.
///
/// Flow:
///   1. Check rate limits via IPasswordRateLimiter
///   2. Load UrlMapping and confirm it is still password-protected
///   3. BCrypt.Verify — constant-time comparison
///   4. On success: log click with server-determined source (provided by endpoint)
/// </summary>
public class VerifyUrlPasswordHandler : IRequestHandler<VerifyUrlPasswordCommand, VerifyUrlPasswordResult>
{
    private readonly IUrlRepository _repo;
    private readonly IPasswordRateLimiter _rateLimiter;

    public VerifyUrlPasswordHandler(
        IUrlRepository repo,
        IPasswordRateLimiter rateLimiter)
    {
        _repo = repo;
        _rateLimiter = rateLimiter;
    }

    public async ValueTask<VerifyUrlPasswordResult> Handle(
        VerifyUrlPasswordCommand request,
        CancellationToken ct)
    {
        // ── Step 1: Rate limiting by IP ────────────────────────────────────
        if (!_rateLimiter.IsAllowed(request.ShortCode, request.IpAddress ?? string.Empty))
        {
            return new VerifyUrlPasswordResult(
                false,
                "Too many incorrect attempts. Please try again later.");
        }

        // ── Step 2: Load the URL mapping ───────────────────────────────────
        var url = await _repo.GetByShortCodeAsync(request.ShortCode);

        if (url is null || url.IsDeleted)
            return new VerifyUrlPasswordResult(false, "Not found.");

        if (!url.IsPasswordProtected || string.IsNullOrEmpty(url.PasswordHash))
            return new VerifyUrlPasswordResult(false, "This link is not password-protected.");

        if (url.ExpirationDate.HasValue && url.ExpirationDate.Value < DateTime.UtcNow)
            return new VerifyUrlPasswordResult(false, "This link has expired.");

        // ── Step 3: BCrypt verify (constant-time) ──────────────────────────
        bool passwordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, url.PasswordHash);

        if (!passwordCorrect)
        {
            // Increment rate-limit counter on wrong password
            _rateLimiter.RecordFailedAttempt(request.ShortCode, request.IpAddress ?? string.Empty);
            return new VerifyUrlPasswordResult(false, "Incorrect password.");
        }

        // ── Step 4: Log the click (source from server-signed token) ────────
        url.ClickCount++;

        var clickLog = new ClickLog
        {
            ShortCode = request.ShortCode,
            VisitorId = Guid.NewGuid().ToString(),
            ClickedAt = DateTime.UtcNow,
            Source = request.Source,           // server-determined by the endpoint, not client
            Country = request.Country,
            Browser = request.Browser,
            OS = request.OS,
            DeviceLanguage = request.DeviceLanguage,
            Referrer = request.Referrer,
            IpAddress = request.IpAddress,
            RawHeaders = request.RawHeaders
        };

        await _repo.UpdateAsync(url);
        await _repo.LogClickAsync(clickLog, ct);

        return new VerifyUrlPasswordResult(true, null);
    }
}
