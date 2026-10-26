using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using TinyBtUrlApi.UseCases.Urls.PreviewUrl;
using TinyBtUrlApi.Web.Extensions;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Preview;

public class ContinueRedirectRequest
{
    public string Token { get; set; } = string.Empty;
}

public class ContinueRedirectEndpoint : Endpoint<ContinueRedirectRequest>
{
    private readonly IMediator _mediator;

    public ContinueRedirectEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/urls/{shortCode}/continue");
        AllowAnonymous();
        Description(x => x.WithTags("Url Management"));
    }

    public override async Task HandleAsync(ContinueRedirectRequest req, CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Short code is required." }, ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Token))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Preview token is required." }, ct);
            return;
        }

        var headers = HttpContext.Request.Headers;
        var ip =
            headers["X-Forwarded-For"].FirstOrDefault() ??
            HttpContext.Connection.RemoteIpAddress?.ToString();

        var command = new ContinueRedirectCommand(
            ShortCode: shortCode,
            Token: req.Token,
            UserAgent: headers["User-Agent"].ToString(),
            OS: headers["sec-ch-ua-platform"].ToString(),
            Country: headers["CF-IPCountry"].ToString(),
            DeviceLanguage: headers["Accept-Language"].ToString(),
            Referrer: headers["Referer"].ToString(),
            DeviceType: headers["sec-ch-ua-mobile"].ToString(),
            IpAddress: ip,
            RawHeaders: headers.ToString()
        );

        var result = await _mediator.Send(command, ct);

        switch (result.Status)
        {
            case ContinueRedirectStatus.InvalidToken:
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case ContinueRedirectStatus.NotFound:
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case ContinueRedirectStatus.Expired:
                HttpContext.Response.StatusCode = StatusCodes.Status410Gone;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case ContinueRedirectStatus.Blocked:
                HttpContext.Response.StatusCode = StatusCodes.Status451UnavailableForLegalReasons;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case ContinueRedirectStatus.Success:
            default:
                await Send.OkAsync(new { redirectTo = result.RedirectTo }, ct);
                return;
        }
    }
}
