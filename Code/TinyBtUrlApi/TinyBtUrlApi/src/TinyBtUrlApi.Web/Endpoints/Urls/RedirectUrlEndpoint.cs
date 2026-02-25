using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.RedirectUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class RedirectUrlEndpoint : EndpointWithoutRequest
{
  private readonly IMediator _mediator;

  public RedirectUrlEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    // Root short url route → TinyURL behavior
    Get("/{shortCode}");

    AllowAnonymous();

    // Important so other APIs work first
    Options(x => x.WithOrder(int.MaxValue));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var shortCode = Route<string>("shortCode");

    if (string.IsNullOrWhiteSpace(shortCode))
    {
      HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
      return;
    }

    var url = await _mediator.Send(new RedirectUrlQuery(shortCode), ct);

    if (url == null)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
      return;
    }

    // ⭐ return 200 JSON (what you want)
    HttpContext.Response.StatusCode = StatusCodes.Status302Found;
    HttpContext.Response.Headers.Location = url.LongUrl;
    await HttpContext.Response.CompleteAsync();
  }
  }
