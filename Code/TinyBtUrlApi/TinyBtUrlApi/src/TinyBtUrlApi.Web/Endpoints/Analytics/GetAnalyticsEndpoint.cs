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

    Roles("User");
  }

  public override async Task HandleAsync(
      GetAnalyticsRequest req,
      CancellationToken ct)
  {
    int userId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var result = await _mediator.Send(
        new GetAnalyticsQuery(userId, req.From, req.To, req.Type),
        ct);

    await Send.OkAsync(result);
  }
}
