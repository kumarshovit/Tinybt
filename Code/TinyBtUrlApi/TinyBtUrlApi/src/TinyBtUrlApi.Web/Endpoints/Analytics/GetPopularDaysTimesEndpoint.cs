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
    Get("/api/analytics/{shortCode}/popular-times");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      GetPopularDaysTimesRequest req,
      CancellationToken ct)
  {
    var result = await _sender.Send(
        new GetPopularDaysTimesQuery(req.ShortCode), ct);
    await Send.OkAsync(result, ct);

  }
}
