using FastEndpoints;
using TinyBtUrlApi.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Password;

public class VerifyAccessAndRedirectEndpoint : EndpointWithoutRequest
{
    private readonly IUrlRepository _repo;
    private readonly IUrlAccessTokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VerifyAccessAndRedirectEndpoint> _logger;

    public VerifyAccessAndRedirectEndpoint(
        IUrlRepository repo,
        IUrlAccessTokenService tokenService,
        IConfiguration configuration,
        ILogger<VerifyAccessAndRedirectEndpoint> logger)
    {
        _repo = repo;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/urls/{shortCode}/access");
        AllowAnonymous();
        Description(x => x.WithTags("Url Management"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");
        var frontendBaseUrl = _configuration["BaseUrl:Domain"];

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            _logger.LogWarning("Access denied: shortCode route parameter is empty.");
            HttpContext.Response.Redirect($"{frontendBaseUrl}/not-found", false);
            await HttpContext.Response.CompleteAsync();
            return;
        }

        // ── Cookie & Query validation ──────────────────────────────────────
        var rawToken = HttpContext.Request.Query["access_token"].ToString();
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            if (!HttpContext.Request.Cookies.TryGetValue("url_access", out rawToken) ||
                string.IsNullOrWhiteSpace(rawToken))
            {
                _logger.LogWarning("Access denied for {ShortCode}: url_access is missing or empty.", shortCode);
                HttpContext.Response.Redirect($"{frontendBaseUrl}/protected/{shortCode}", false);
                await HttpContext.Response.CompleteAsync();
                return;
            }
        }

        // Unprotect validates MAC, expiry, shortCode match, and clock-skew.
        var payload = _tokenService.TryUnprotect(rawToken, shortCode);

        // ── Consume/clear the cookie immediately (one-time-use semantics) ──
        HttpContext.Response.Cookies.Delete("url_access", new CookieOptions
        {
            Path = $"/api/urls/{shortCode}/access",
            Secure = HttpContext.Request.IsHttps,
            SameSite = HttpContext.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax
        });

        if (payload is null)
        {
            _logger.LogWarning("Access denied for {ShortCode}: TryUnprotect returned null. The token was tampered, expired, or failed deserialization.", shortCode);
            HttpContext.Response.Redirect($"{frontendBaseUrl}/protected/{shortCode}", false);
            await HttpContext.Response.CompleteAsync();
            return;
        }

        // Validate source is a known value (defence-in-depth)
        if (payload.Source != "Direct" && payload.Source != "QRCode")
        {
            _logger.LogWarning("Access denied for {ShortCode}: Bad token source '{Source}'.", shortCode, payload.Source);
            HttpContext.Response.Redirect($"{frontendBaseUrl}/not-found", false);
            await HttpContext.Response.CompleteAsync();
            return;
        }

        // ── Load URL mapping ───────────────────────────────────────────────
        var url = await _repo.GetByShortCodeAsync(shortCode);

        if (url is null || url.IsDeleted)
        {
            _logger.LogWarning("Access denied for {ShortCode}: URL not found in DB.", shortCode);
            HttpContext.Response.Redirect($"{frontendBaseUrl}/not-found", false);
            await HttpContext.Response.CompleteAsync();
            return;
        }

        if (url.ExpirationDate.HasValue && url.ExpirationDate.Value < DateTime.UtcNow)
        {
            _logger.LogWarning("Access denied for {ShortCode}: URL expired.", shortCode);
            HttpContext.Response.Redirect($"{frontendBaseUrl}/expired-link", false);
            await HttpContext.Response.CompleteAsync();
            return;
        }

        _logger.LogInformation("Access granted for {ShortCode}. Redirecting to long URL.", shortCode);
        HttpContext.Response.StatusCode = StatusCodes.Status302Found;
        HttpContext.Response.Headers.Location = url.LongUrl;
        await HttpContext.Response.CompleteAsync();
    }
}
