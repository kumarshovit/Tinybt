//using FastEndpoints;
//using Mediator;
//using TinyBtUrlApi.UseCases.Urls.Analytics;
//using TinyBtUrlApi.Core.DTOs;

//namespace TinyBtUrlApi.Web.Endpoints.Analytics;

//public class GetClicksByBrowserEndpoint(IMediator mediator)
//    : Endpoint<ClicksByBrowserRequest, List<ClicksByBrowserDto>>
//{
//  public override void Configure()
//  {
//    Get("/api/analytics/clicks-by-browser");
//    AllowAnonymous();
//  }

//  public override async Task HandleAsync(
//      ClicksByBrowserRequest req,
//      CancellationToken ct)
//  {
//    var result = await mediator.Send(
//        new GetClicksByBrowserQuery(
//            req.StartDate,
//            req.EndDate),
//        ct);

//    await Send.OkAsync(result, ct);
//  }
//}

using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksByBrowserEndpoint
    : Endpoint<ClicksByBrowserRequest, List<ClicksByBrowserDto>>
{
  private readonly IMediator _mediator;

  public GetClicksByBrowserEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/analytics/clicks-by-browser");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      ClicksByBrowserRequest req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(
        new GetClicksByBrowserQuery(
            req.StartDate,
            req.EndDate),
        ct);

    await Send.OkAsync(result, ct);
  }
}
