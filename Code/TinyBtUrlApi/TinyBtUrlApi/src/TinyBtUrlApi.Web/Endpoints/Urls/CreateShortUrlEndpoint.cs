using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Configuration;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using System.Security.Claims;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class CreateShortUrlEndpoint
    : Endpoint<CreateShortUrlCommand>
{
  private readonly IMediator _mediator;
  private readonly IConfiguration _config;

  public CreateShortUrlEndpoint(IMediator mediator, IConfiguration config)
  {
    _mediator = mediator;
    _config = config;
  }

  public override void Configure()
  {
    Post("/api/urls");
    AllowAnonymous();
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(
      CreateShortUrlCommand req,
      CancellationToken ct)
  {
    int? userId = null;
    if (HttpContext.User.Identity?.IsAuthenticated == true)
    {
      var claim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (claim != null)
      {
        userId = int.Parse(claim);
      }
    }

    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

    var result = await _mediator.Send(
        new CreateShortUrlCommand(
            req.LongUrl,
            req.CustomAlias,
            req.ExpirationDate,
            userId,
            ipAddress
        ),
        ct
    );

    // 🔴 Validation fail case
    if (!result.Success)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

      await HttpContext.Response.WriteAsJsonAsync(new
      {
        message = result.Message
      }, ct);

      return;
    }

    // ✅ Success case
    var baseUrl = HttpContext.Request.Host.Host.Contains("localhost")
      ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
      : (_config["BaseUrl:ShortUrlDomain"] ?? "https://link.bt");
    var shortUrl = $"{baseUrl}/{result.ShortCode}";

    HttpContext.Response.StatusCode = StatusCodes.Status201Created;
    HttpContext.Response.Headers.Location = shortUrl;

    await HttpContext.Response.WriteAsJsonAsync(new
    {
      result.Id,
      result.ShortCode,
      ShortUrl = shortUrl,
      result.LongUrl,
      result.ExpirationDate,
      result.CreatedAt
    }, ct);
  }
}
