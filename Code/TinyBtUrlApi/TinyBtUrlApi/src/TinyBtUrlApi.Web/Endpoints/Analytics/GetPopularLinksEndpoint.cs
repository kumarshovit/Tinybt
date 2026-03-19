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

    Roles("User", "Admin");
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

    int loggedInUserId = int.Parse(userIdClaim.Value);

    // ✅ Check if Admin
    bool isAdmin = User.IsInRole("Admin");

    // 🔥 MAIN FIX
    int finalUserId = isAdmin && req.UserId.HasValue
        ? req.UserId.Value
        : loggedInUserId;

    var result = await _mediator.Send(
        new GetPopularLinksByUserQuery(
            finalUserId,   // ✅ FIXED
            req.From,
            req.To,
            req.Link,
            req.Tag
        ),
        ct);

    await Send.OkAsync(result);
  }
}
