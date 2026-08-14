using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.RedirectUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class RedirectUrlEndpoint : EndpointWithoutRequest
{
  private readonly IMediator _mediator;
  private readonly IConfiguration _configuration;
  private readonly IUrlAccessTokenService _tokenService;

  public RedirectUrlEndpoint(
    IMediator mediator,
    IConfiguration configuration,
    IUrlAccessTokenService tokenService)
  {
    _mediator = mediator;
    _configuration = configuration;
    _tokenService = tokenService;
  }

  public override void Configure()
  {
    Get("/{shortCode}");
    AllowAnonymous();
    Options(x => x.WithOrder(int.MaxValue));
    Description(x => x.WithTags("Url Management"));
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

    var ip =
        headers["X-Forwarded-For"].FirstOrDefault() ??
        HttpContext.Connection.RemoteIpAddress?.ToString();

    var query = new RedirectUrlQuery(
        shortCode,
        headers["User-Agent"].ToString(),
        headers["sec-ch-ua-platform"].ToString(),
        headers["CF-IPCountry"].ToString(),
        headers["Accept-Language"].ToString(),
        headers["Referer"].ToString(),
        headers["sec-ch-ua-mobile"].ToString(),
        ip,
        headers.ToString()
    );

    var result = await _mediator.Send(query, ct);
    var frontendBaseUrl = _configuration["BaseUrl:Domain"];

    if (result.Status == RedirectStatus.NotFound)
    {
      HttpContext.Response.Redirect(
          $"{frontendBaseUrl}/not-found",
          false,
          false
      );
      await HttpContext.Response.CompleteAsync();
      return;
    }

    if (result.Status == RedirectStatus.Expired)
    {
      HttpContext.Response.Redirect(
          $"{frontendBaseUrl}/expired-link",
          false,
          false
      );
      await HttpContext.Response.CompleteAsync();
      return;
    }

    // 🔐 Password-protected: issue a signed token encoding source="Direct".
    // The token is opaque to the browser; no destination URL is revealed.
    if (result.Status == RedirectStatus.PasswordProtected)
    {
      var token = _tokenService.Protect(shortCode, "Direct");
      HttpContext.Response.Redirect(
          $"{frontendBaseUrl}/protected/{shortCode}?token={Uri.EscapeDataString(token)}",
          false,
          false
      );
      await HttpContext.Response.CompleteAsync();
      return;
    }

    HttpContext.Response.StatusCode = StatusCodes.Status302Found;
    HttpContext.Response.Headers.Location = result.Url!.LongUrl;
    await HttpContext.Response.CompleteAsync();
  }
}
