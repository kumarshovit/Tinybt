using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksByDeviceTypeEndpoint(IMediator mediator)
    : EndpointWithoutRequest<List<ClicksByDeviceTypeDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/device-type");
    AllowAnonymous();
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var startDate = Query<DateTime?>("startDate", false);
    var endDate = Query<DateTime?>("endDate", false);

    var result = await mediator.Send(
        new GetClicksByDeviceTypeQuery(startDate, endDate),
        ct);

    HttpContext.Response.StatusCode = StatusCodes.Status200OK;
    await HttpContext.Response.WriteAsJsonAsync(result, ct);
  }
}
