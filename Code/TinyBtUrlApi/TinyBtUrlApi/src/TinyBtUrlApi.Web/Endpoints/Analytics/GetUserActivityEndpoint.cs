using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;



public class GetUserActivityEndpoint : Endpoint<GetUserActivityRequest>
{
  private readonly IMediator _mediator;

  public GetUserActivityEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/admin/users/{UserId}/activity");
    Roles("Admin");
  }

  public override async Task HandleAsync(GetUserActivityRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetUserActivityQuery(req.UserId), ct);

    await Send.OkAsync(result, ct);
  }
}
