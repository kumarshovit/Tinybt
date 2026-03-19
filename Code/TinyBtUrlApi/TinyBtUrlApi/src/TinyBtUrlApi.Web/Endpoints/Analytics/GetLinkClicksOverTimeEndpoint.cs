using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetLinkClicksOverTimeEndpoint
    : Endpoint<GetLinkClicksOverTimeQuery, List<ClickOverTimeDto>>
{
  private readonly IMediator _mediator;

  public GetLinkClicksOverTimeEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/analytics/{shortCode}/clicks-over-time");
    Roles("User");
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(
      GetLinkClicksOverTimeQuery req,
      CancellationToken ct)
  {
    req.ShortCode = Route<string>("shortCode")!;

    var result = await _mediator.Send(req);

    await Send.OkAsync(result, ct);
  }
}
