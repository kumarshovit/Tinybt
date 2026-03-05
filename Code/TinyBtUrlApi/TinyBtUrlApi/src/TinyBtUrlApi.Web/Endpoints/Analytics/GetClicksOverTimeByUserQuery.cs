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

    Roles("User");
  }

  public override async Task HandleAsync(
      ClicksOverTimeRequestUser req,
      CancellationToken ct)
  {
    int userId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var result = await _mediator.Send(
        new GetClicksOverTimeByUserQuery(
            userId,
            req.From,
            req.To),
        ct);

    await Send.OkAsync(result);
  }
}
