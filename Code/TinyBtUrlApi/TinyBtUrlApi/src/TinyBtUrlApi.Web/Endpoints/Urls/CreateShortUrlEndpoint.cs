using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

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
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      CreateShortUrlCommand req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

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
