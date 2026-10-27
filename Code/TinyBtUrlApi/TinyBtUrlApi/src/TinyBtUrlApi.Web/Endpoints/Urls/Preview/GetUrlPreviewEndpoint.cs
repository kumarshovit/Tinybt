using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.UseCases.Urls.PreviewUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Preview;

public class GetUrlPreviewEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public GetUrlPreviewEndpoint(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Get("/api/urls/{shortCode}/preview");
        AllowAnonymous();
        Description(x => x.WithTags("Url Management"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        HttpContext.Response.Headers.CacheControl = "no-store";

        var shortCode = Route<string>("shortCode");
        var token = Query<string>("t");

        if (string.IsNullOrWhiteSpace(shortCode))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Short code is required." }, ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Preview token is missing. Please visit the original short link." }, ct);
            return;
        }

        var result = await _mediator.Send(new GetUrlPreviewQuery(shortCode, token), ct);

        switch (result.Status)
        {
            case GetUrlPreviewStatus.InvalidToken:
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case GetUrlPreviewStatus.NotFound:
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case GetUrlPreviewStatus.Expired:
                HttpContext.Response.StatusCode = StatusCodes.Status410Gone;
                await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
                return;

            case GetUrlPreviewStatus.Blocked:
                // Per requirement: If malicious/blocked, do not create or return destination preview data
                HttpContext.Response.StatusCode = StatusCodes.Status451UnavailableForLegalReasons;
                await HttpContext.Response.WriteAsJsonAsync(new
                {
                    message = result.Message ?? "This link has been blocked as malicious or unsafe.",
                    isBlocked = true
                }, ct);
                return;

            case GetUrlPreviewStatus.Success:
            default:
                string? absoluteScreenshotUrl = null;
                if (result.ScreenshotStatus == ScreenshotStatus.Ready && !string.IsNullOrWhiteSpace(result.ScreenshotUrl))
                {
                    string apiBaseUrl;
                    if (HttpContext.Request.Host.Host.Contains("localhost"))
                    {
                        apiBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                    }
                    else
                    {
                        var configDomain = _configuration["BaseUrl:ApiDomain"] ?? _configuration["BaseUrl:ShortUrlDomain"];
                        apiBaseUrl = !string.IsNullOrWhiteSpace(configDomain)
                            ? configDomain.TrimEnd('/')
                            : $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                    }

                    absoluteScreenshotUrl = $"{apiBaseUrl.TrimEnd('/')}/{result.ScreenshotUrl.TrimStart('/')}";
                }

                await Send.OkAsync(new
                {
                    shortCode = result.ShortCode,
                    destinationDomain = result.DestinationDomain,
                    destinationUrl = result.DestinationUrl,
                    isSafe = true,
                    expirationDate = result.ExpirationDate,
                    screenshotUrl = absoluteScreenshotUrl,
                    screenshotStatus = result.ScreenshotStatus.ToString()
                }, ct);
                return;
        }
    }
}
