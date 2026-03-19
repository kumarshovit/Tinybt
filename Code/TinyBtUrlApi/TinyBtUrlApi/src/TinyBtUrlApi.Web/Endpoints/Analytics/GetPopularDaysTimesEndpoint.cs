//using FastEndpoints;
//using Mediator;
//using TinyBtUrlApi.UseCases.Urls.Analytics;

//namespace TinyBtUrlApi.Web.Endpoints.Analytics;

//public class GetPopularDaysTimesEndpoint
//    : Endpoint<GetPopularDaysTimesRequest, object>
//{
//  private readonly ISender _sender;

//  public GetPopularDaysTimesEndpoint(ISender sender)
//  {
//    _sender = sender;
//  }

//  public override void Configure()
//  {
//    Get("/api/analytics/{shortCode}/popular-times");
//    AllowAnonymous();
//  }

//  public override async Task HandleAsync(
//      GetPopularDaysTimesRequest req,
//      CancellationToken ct)
//  {
//    var result = await _sender.Send(
//        new GetPopularDaysTimesQuery(req.ShortCode), ct);
//    await Send.OkAsync(result, ct);

//  }
//}


using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetPopularDaysTimesEndpoint
    : Endpoint<GetPopularDaysTimesRequest, object>
{
  private readonly ISender _sender;

  public GetPopularDaysTimesEndpoint(ISender sender)
  {
    _sender = sender;
  }

  public override void Configure()
  {
    Get("/api/analytics/popular-times"); // no shortcode in URL
    AllowAnonymous();
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(
      GetPopularDaysTimesRequest req,
      CancellationToken ct)
  {
    var result = await _sender.Send(
        new GetPopularDaysTimesQuery(
            req.StartDate,
            req.EndDate),
        ct);

    await Send.OkAsync(result, ct);
  }
}
