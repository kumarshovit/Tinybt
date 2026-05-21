using FastEndpoints;
using Mediator;
using System.Security.Claims;
using TinyBtUrlApi.UseCases.Urls.DeleteUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class DeleteUrlEndpoint
    : EndpointWithoutRequest<bool>
{
  private readonly IMediator _mediator;

  public DeleteUrlEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/urls/{id}");
    Roles("User", "Admin");
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");
    var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var isAdmin = HttpContext.User.IsInRole("Admin");

    var result = await _mediator.Send(new DeleteUrlCommand(id, userId, isAdmin), ct);

    if (!result)
    {
      await Send.NotFoundAsync(ct);
      return;
    }

    await Send.OkAsync(result, ct);
  }
}
