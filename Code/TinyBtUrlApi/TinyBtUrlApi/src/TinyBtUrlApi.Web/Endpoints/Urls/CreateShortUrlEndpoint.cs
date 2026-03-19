using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using System.Security.Claims;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class CreateShortUrlEndpoint
    : Endpoint<CreateShortUrlCommand>
{
  private readonly IMediator _mediator;

  public CreateShortUrlEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/urls");
    Roles("User", "Admin");
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(
      CreateShortUrlCommand req,
      CancellationToken ct)
  {
    var userId = int.Parse(
        HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );

    var result = await _mediator.Send(
        new CreateShortUrlCommand(
            req.LongUrl,
            req.CustomAlias,
            req.ExpirationDate,
            userId
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
    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
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
