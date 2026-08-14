using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.VerifyUrlPassword;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Password;

public class VerifyUrlPasswordRequest
{
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class VerifyUrlPasswordEndpoint : Endpoint<VerifyUrlPasswordRequest>
{
    private readonly IMediator _mediator;
    private readonly IUrlAccessTokenService _tokenService;

    public VerifyUrlPasswordEndpoint(
        IMediator mediator,
        IUrlAccessTokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    public override void Configure()
    {
        Post("/api/urls/{shortCode}/verify-password");
        AllowAnonymous();
        Description(x => x.WithTags("Url Management"));
    }

    public override async Task HandleAsync(VerifyUrlPasswordRequest req, CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        
        if (string.IsNullOrWhiteSpace(req.Password) || string.IsNullOrWhiteSpace(req.Token))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Password and token are required." }, ct);
            return;
        }

        // ── Validate initial access token (from redirect) ──────────────
        var payload = _tokenService.TryUnprotect(req.Token, shortCode);
        if (payload is null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(
                new { message = "Invalid or expired access token. Please visit the link again." }, ct);
            return;
        }

        var headers = HttpContext.Request.Headers;
        var ip =
            headers["X-Forwarded-For"].FirstOrDefault() ??
            HttpContext.Connection.RemoteIpAddress?.ToString();

        // Pass the extracted Source so the handler can just do business logic
        var command = new VerifyUrlPasswordCommand(
            ShortCode: shortCode,
            Password: req.Password,
            Source: payload.Source,
            Browser: headers["User-Agent"].ToString(),
            OS: headers["sec-ch-ua-platform"].ToString(),
            Country: headers["CF-IPCountry"].ToString(),
            DeviceLanguage: headers["Accept-Language"].ToString(),
            Referrer: headers["Referer"].ToString(),
            DeviceType: headers["sec-ch-ua-mobile"].ToString(),
            IpAddress: ip,
            RawHeaders: headers.ToString()
        );

        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
            return;
        }

        // ── Issue the new access token ──────────────
        var newAccessToken = _tokenService.Protect(shortCode, payload.Source);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = HttpContext.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = $"/api/urls/{shortCode}/access",
            MaxAge = TimeSpan.FromMinutes(5)
        };

        HttpContext.Response.Cookies.Append("url_access", newAccessToken, cookieOptions);

        var apiBase = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        var encodedToken = Uri.EscapeDataString(newAccessToken);
        await HttpContext.Response.WriteAsJsonAsync(new
        {
            redirectTo = $"{apiBase}/api/urls/{shortCode}/access?access_token={encodedToken}"
        }, ct);
    }
}
