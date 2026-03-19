using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;



//public class GetUserActivityEndpoint : Endpoint<GetUserActivityRequest>
//{
//  private readonly IMediator _mediator;

//  public GetUserActivityEndpoint(IMediator mediator)
//  {
//    _mediator = mediator;
//  }
public class GetUserActivityEndpoint(IMediator mediator)
    : Endpoint<GetUserActivityRequest, List<UserActivityDto>>
{

  public override void Configure()
  {
    Get("/api/analytics/users/{UserId}/activity");
    Roles("Admin");
    Description(x => x.WithTags("Analytics"));
  }

  public override async Task HandleAsync(GetUserActivityRequest req, CancellationToken ct)
  {
    var result = await mediator.Send(new GetUserActivityQuery(req.UserId), ct);

    await Send.OkAsync(result, ct);
  }
}
