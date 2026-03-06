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
    int userId = int.Parse(
        User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    var result = await _mediator.Send(
        new GetPopularLinksByUserQuery(
            userId,
            req.From,
            req.To),
        ct);

    await Send.OkAsync(result);
  }
}
