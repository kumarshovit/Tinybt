using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetUserDashboardEndpoint
    : Endpoint<UserDashboardRequest, DashboardOverviewDto>
{
  private readonly IMediator _mediator;

  public GetUserDashboardEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/analytics/user-dashboard");

    // login required
    Roles("User");
  }

  public override async Task HandleAsync(
      UserDashboardRequest req,
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
        new GetUserDashboardQuery(userId, req.From, req.To), ct);

    await Send.OkAsync(result, cancellation: ct);
  }
}
