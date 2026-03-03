using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksOverTimeEndpoint(IMediator mediator)
    : Endpoint<ClicksOverTimeRequest, List<ClickOverTimeDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/clicks-over-time");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      ClicksOverTimeRequest req,
      CancellationToken ct)
  {
    var result = await mediator.Send(
        new GetClicksOverTimeQuery(
            req.StartDate,
            req.EndDate,
            req.ViewType),
        ct);

    await Send.OkAsync(result, ct);
  }
}
