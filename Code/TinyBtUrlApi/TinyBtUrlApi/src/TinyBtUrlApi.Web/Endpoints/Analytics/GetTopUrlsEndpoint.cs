using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetTopUrlsEndpoint(IMediator mediator)
    : Endpoint<GetTopUrlsRequest, List<TopUrlDto>>
{
  public override void Configure()
  {
    Get("/api/analytics/top-urls");
    AllowAnonymous();
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(
      GetTopUrlsRequest req,
      CancellationToken ct)
  {
    var result = await mediator.Send(
        new GetTopUrlsQuery(req.TopCount),
        ct);

    await Send.OkAsync(result, ct);
  }
}
