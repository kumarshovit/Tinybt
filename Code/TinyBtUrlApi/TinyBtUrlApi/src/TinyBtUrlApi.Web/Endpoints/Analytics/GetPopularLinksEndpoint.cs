using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetPopularLinksEndpoint
    : Endpoint<PopularLinksRequest, List<AnalyticsItemDto>>
{
  private readonly IMediator _mediator;

  public GetPopularLinksEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/analytics/user/popular-links");

    Roles("User");
  }

  public override async Task HandleAsync(
      PopularLinksRequest req,
      CancellationToken ct)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int userId = int.Parse(userIdClaim.Value);

    var result = await _mediator.Send(
        new GetPopularLinksByUserQuery(
            userId,
            req.From,
            req.To, req.Link,
    req.Tag),
        ct);

    await Send.OkAsync(result);
  }
}
