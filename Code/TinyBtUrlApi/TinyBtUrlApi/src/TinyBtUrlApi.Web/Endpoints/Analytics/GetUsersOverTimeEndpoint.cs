using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.Analytics;
using TinyBtUrlApi.Core.DTOs;

public class GetUsersOverTimeEndpoint
    : Endpoint<GetUsersOverTimeRequest, List<UsersOverTimeDto>>
{
  private readonly IMediator _mediator;

  public GetUsersOverTimeEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/analytics/users-over-time");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      GetUsersOverTimeRequest req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(
        new GetUsersOverTimeQuery(
            req.start,
            req.end,
            req.viewType),
        ct);

    await Send.OkAsync(result);
  }
}
