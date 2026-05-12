using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.GetAllUrls;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;
using TinyBtUrlApi.Web.Endpoints.Urls.Responses;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class GetUserUrlsByAdminEndpoint
    : Endpoint<GetUrlsRequest, List<UrlResponse>>
{
  private readonly IMediator _mediator;

  public GetUserUrlsByAdminEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/admin/urls"); // ✅ NEW ROUTE
    Roles("Admin");
  }

  public override async Task HandleAsync(GetUrlsRequest req, CancellationToken ct)
  {
    var claim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int loggedInUserId = int.Parse(claim.Value);

    int userId = req.UserId ?? loggedInUserId;

    var result = await _mediator.Send(new GetAllUrlsQuery(userId), ct);
    var baseUrl = HttpContext.Request.Host.Host.Contains("localhost")
        ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
        : "https://link.bt";

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
}
