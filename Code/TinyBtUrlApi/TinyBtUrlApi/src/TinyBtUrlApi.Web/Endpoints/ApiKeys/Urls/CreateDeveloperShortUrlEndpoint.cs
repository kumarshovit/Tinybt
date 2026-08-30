using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using TinyBtUrlApi.Web.Auth.ApiKey;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Requests;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Responses;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls;

public class CreateDeveloperShortUrlEndpoint
    : Endpoint<CreateDeveloperShortUrlRequest, CreateDeveloperShortUrlResponse>
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _config;

    public CreateDeveloperShortUrlEndpoint(IMediator mediator, IConfiguration config)
    {
        _mediator = mediator;
        _config = config;
    }

    public override void Configure()
    {
        Post("/api/v1/links");
        AuthSchemes(ApiKeyAuthenticationOptions.DefaultScheme);
        Options(x => x.RequireRateLimiting(RateLimitConfigs.DeveloperApiUrlCreationPolicy));
        Description(x => x
            .WithTags("Developer API - Links")
            .WithSummary("Create a short URL")
            .WithDescription("Creates a new short URL. Requires a valid Developer API key."));
    }

    public override async Task HandleAsync(CreateDeveloperShortUrlRequest req, CancellationToken ct)
    {
        var claim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claim, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Push directly into the existing business logic layer
        var command = new CreateShortUrlCommand(
            req.LongUrl,
            req.CustomAlias,
            req.ExpirationDate,
            userId,           // Hard override: strictly from authenticated ClaimsPrincipal
            ipAddress,        // Hard override: strictly from remote connection
            null,             // No CAPTCHA for Developer API
            req.Password
        );

        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsJsonAsync(new { message = result.Message }, ct);
            return;
        }

        var baseUrl = HttpContext.Request.Host.Host.Contains("localhost")
          ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
          : (_config["BaseUrl:ShortUrlDomain"] ?? "https://link.bt");
        var shortUrl = $"{baseUrl}/{result.ShortCode}";

        var response = new CreateDeveloperShortUrlResponse
        {
            Id = result.Id ?? 0,
            ShortCode = result.ShortCode,
            ShortUrl = shortUrl,
            LongUrl = result.LongUrl,
            ExpirationDate = result.ExpirationDate,
            CreatedAt = result.CreatedAt ?? System.DateTime.UtcNow,
            IsPasswordProtected = result.IsPasswordProtected
        };

        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        HttpContext.Response.Headers.Location = shortUrl;

        await HttpContext.Response.WriteAsJsonAsync(response, ct);
    }
}