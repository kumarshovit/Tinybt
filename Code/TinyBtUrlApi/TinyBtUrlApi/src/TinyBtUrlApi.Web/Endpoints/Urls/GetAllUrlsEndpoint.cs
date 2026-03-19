using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.GetAllUrls;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Web.Endpoints.Urls.Responses;
using System.Security.Claims;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class GetAllUrlsEndpoint
    : EndpointWithoutRequest<List<UrlResponse>>
{
  private readonly IMediator _mediator;

  public GetAllUrlsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/urls");
    Roles("User", "Admin");
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    try
    {
      var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userIdClaim))
      {
        await Send.UnauthorizedAsync(ct);
        return;
      }

      var userId = int.Parse(userIdClaim);

      var result = await _mediator.Send(new GetAllUrlsQuery(userId), ct);

      var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

      var response = result.Select(x => new UrlResponse
      {
        Id = x.Id,
        LongUrl = x.LongUrl,
        ShortCode = x.ShortCode,
        ShortUrl = $"{baseUrl}/{x.ShortCode}",
        ExpirationDate = x.ExpirationDate,
        ClickCount = x.ClickCount,
        Tags = x.Tags
      }).ToList();

      await Send.OkAsync(response, ct);
    }
    catch (OperationCanceledException)
    {
      // ignore
    }
  }
}
