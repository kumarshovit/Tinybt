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
    // Important: this must NOT conflict with /api/urls routes
    Get("/r/{shortCode}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var shortCode = Route<string>("shortCode");

    if (string.IsNullOrWhiteSpace(shortCode))
    {
      await Send.NotFoundAsync();
      return;
    }

    var result = await _mediator.Send(
        new RedirectUrlQuery(shortCode),
        ct);

    if (result is null)
    {
      await Send.NotFoundAsync();
      return;
    }

    var acceptHeader = HttpContext.Request.Headers["Accept"].ToString();

    // If request is from Swagger (JSON request)
    if (acceptHeader.Contains("application/json"))
    {
      await Send.OkAsync(new
      {
        longUrl = result.LongUrl
      }, ct);
    }
    else
    {
      // If request is from browser
      Response.Redirect(result.LongUrl, false);
    }
  }
}
