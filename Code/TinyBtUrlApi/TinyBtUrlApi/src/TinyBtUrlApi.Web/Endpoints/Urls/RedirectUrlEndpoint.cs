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
    Get("/{shortCode}");
    AllowAnonymous();
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

    var headers = HttpContext.Request.Headers;

    var query = new RedirectUrlQuery(
        shortCode,
        headers["User-Agent"].ToString(),               // Browser
        headers["sec-ch-ua-platform"].ToString(),       // OS
        headers["CF-IPCountry"].ToString(),             // Country (if behind proxy)
        headers["Accept-Language"].ToString(),          // Language
        headers["Referer"].ToString(),                  // Referrer
        headers["sec-ch-ua-mobile"].ToString(),         // DeviceType
        HttpContext.Connection.RemoteIpAddress?.ToString(), // IP
        headers.ToString()                              // RawHeaders
    );

    var url = await _mediator.Send(query, ct);

    if (url == null)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
      return;
    }

    HttpContext.Response.StatusCode = StatusCodes.Status302Found;
    HttpContext.Response.Headers.Location = url.LongUrl;
    await HttpContext.Response.CompleteAsync();
  }
}
