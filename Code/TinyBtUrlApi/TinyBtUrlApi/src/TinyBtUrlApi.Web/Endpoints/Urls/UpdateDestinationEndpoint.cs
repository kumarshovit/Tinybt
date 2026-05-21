using FastEndpoints;
using Mediator;
using System.Security.Claims;
using TinyBtUrlApi.UseCases.Urls.UpdateDestination;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class UpdateDestinationEndpoint
    : Endpoint<UpdateDestinationRequest, bool>
{
  private readonly IMediator _mediator;

  public UpdateDestinationEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/urls/{id}/destination");
    Roles("User", "Admin");
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(UpdateDestinationRequest req, CancellationToken ct)
  {
    var id = Route<int>("id");
    var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var isAdmin = HttpContext.User.IsInRole("Admin");

    var command = new UpdateDestinationCommand(id, req.NewLongUrl, userId, isAdmin);

    var result = await _mediator.Send(command, ct);

    if (!result)
    {
      await Send.NotFoundAsync();
      return;
    }

    await Send.OkAsync(true);
  }
}
