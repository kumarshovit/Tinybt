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

    int userId = int.Parse(userIdClaim.Value);

    var result = await _mediator.Send(
        new GetClicksOverTimeByUserQuery(
            userId,
            req.From,
            req.To, req.Link,
    req.Tag),
        ct);

    await Send.OkAsync(result);
  }
}
