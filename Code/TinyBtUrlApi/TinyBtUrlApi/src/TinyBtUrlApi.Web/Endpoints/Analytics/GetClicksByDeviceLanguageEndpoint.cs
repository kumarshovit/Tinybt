using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksByDeviceLanguageEndpoint(IMediator mediator)
    : EndpointWithoutRequest<List<ClicksByDeviceLanguageDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/clicks-by-language");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await mediator.Send(
        new GetClicksByDeviceLanguageQuery(),
        ct);

    await Send.OkAsync(result, ct);
  }
}
