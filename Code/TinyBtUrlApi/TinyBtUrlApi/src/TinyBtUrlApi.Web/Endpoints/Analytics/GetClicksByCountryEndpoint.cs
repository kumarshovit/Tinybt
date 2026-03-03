using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksByCountryEndpoint(IMediator mediator)
    : Endpoint<ClicksByCountryRequest, List<ClicksByCountryDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/clicks-by-country");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      ClicksByCountryRequest req,
      CancellationToken ct)
  {
    var result = await mediator.Send(
        new GetClicksByCountryQuery(
            req.StartDate,
            req.EndDate),
        ct);

    await Send.OkAsync(result, ct);
  }
}
