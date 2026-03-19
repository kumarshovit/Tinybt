using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public sealed class GetDashboardOverviewEndpoint
    : Endpoint<GetDashboardOverviewRequest, DashboardOverviewDto>
{
  private readonly IMediator _mediator;

  public GetDashboardOverviewEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/analytics/dashboard");
    AllowAnonymous();
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(
      GetDashboardOverviewRequest req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(
        new GetDashboardOverviewQuery
        {
          From = req.From,
          To = req.To
        }, ct);

    await Send.OkAsync(result, ct);
  }
}
