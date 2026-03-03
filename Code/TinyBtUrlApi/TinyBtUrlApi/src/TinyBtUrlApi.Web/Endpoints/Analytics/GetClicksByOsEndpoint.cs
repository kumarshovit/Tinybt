using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksByOsEndpoint(IMediator mediator)
    : EndpointWithoutRequest<List<ClicksByOsDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/clicks-by-os");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await mediator.Send(
        new GetClicksByOsQuery(),
        ct);

    await Send.OkAsync(result, ct);
  }
}
