using System.Security.Claims;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetAnalyticsEndpoint
    : Endpoint<GetAnalyticsRequest, List<AnalyticsItemDto>>
{
  private readonly IMediator _mediator;

  public GetAnalyticsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/analytics/breakdown");

    Roles("User", "Admin");
  }


  public override async Task HandleAsync(
    GetAnalyticsRequest req,
    CancellationToken ct)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int loggedInUserId = int.Parse(userIdClaim.Value);

    // 🔥 MAIN FIX
    int finalUserId = req.UserId ?? loggedInUserId;

    var result = await _mediator.Send(
        new GetAnalyticsQuery(
            finalUserId,   // ✅ use selected user if present
            req.From,
            req.To,
            req.Type,
            req.Link,
            req.Tag
        ),
        ct);

    await Send.OkAsync(result);
  }
}
