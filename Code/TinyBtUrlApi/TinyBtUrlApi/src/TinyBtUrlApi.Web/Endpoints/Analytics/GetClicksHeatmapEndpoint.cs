using FastEndpoints;
using Mediator;
using System.Security.Claims;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksHeatmapEndpoint : EndpointWithoutRequest
{
  private readonly IMediator mediator;

  public GetClicksHeatmapEndpoint(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public override void Configure()
  {
    Get("/analytics/heatmap");

    // Require logged-in user
    AuthSchemes("Bearer");
    Description(x => x.WithTags("Analytics (User specific)"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
      await Send.UnauthorizedAsync();
      return;
    }

    var userId = int.Parse(userIdClaim.Value);

    var start = DateTime.UtcNow.AddDays(-30);
    var end = DateTime.UtcNow;

    var data = await mediator.Send(
        new GetClicksHeatmapQuery(userId, start, end),
        ct);

    await Send.OkAsync(data);
  }
}
