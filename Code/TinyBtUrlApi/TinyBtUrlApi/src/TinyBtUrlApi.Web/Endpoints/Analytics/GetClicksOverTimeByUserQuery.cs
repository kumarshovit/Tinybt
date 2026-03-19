using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksOverTimeUserEndpoint
    : Endpoint<ClicksOverTimeRequestUser, List<AnalyticsItemDto>>
{
  private readonly IMediator _mediator;

  public GetClicksOverTimeUserEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {

    Post("/analytics/user/clicks-over-time");
    Roles("User", "Admin"); // 👈 UPDATED
    Description(x => x.WithTags("Analytics (User specific)"));
  }

  public override async Task HandleAsync(
      ClicksOverTimeRequestUser req,
      CancellationToken ct)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int currentUserId = int.Parse(userIdClaim.Value);

    // 🔥 Detect role
    bool isAdmin = User.IsInRole("Admin");

    // 🔥 Decide target user
    int targetUserId;

    if (isAdmin && req.UserId.HasValue)
    {
      targetUserId = req.UserId.Value;
    }
    else
    {
      targetUserId = currentUserId;
    }

    // 🔐 Security check
    if (!isAdmin && req.UserId.HasValue)
    {
      await Send.ForbiddenAsync(ct);
      return;
    }

    var result = await _mediator.Send(
        new GetClicksOverTimeByUserQuery(
            targetUserId,
            req.From,
            req.To,
            req.Link,
            req.Tag),
        ct);

    await Send.OkAsync(result);
  }
}
