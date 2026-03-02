using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;


namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetTotalClicksEndpoint(IMediator mediator)
    : EndpointWithoutRequest<object>
{
  public override void Configure()
  {
    Get("/api/analytics/total-clicks");
    AllowAnonymous(); // remove if you want auth
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var totalClicks = await mediator.Send(new GetTotalClicksQuery(), ct);

    await Send.OkAsync(new
    {
      totalClicks
    }, ct);
  }
}
