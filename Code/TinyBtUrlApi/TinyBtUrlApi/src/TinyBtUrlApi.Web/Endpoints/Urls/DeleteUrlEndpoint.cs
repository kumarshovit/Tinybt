using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.DeleteUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class DeleteUrlEndpoint
    : Endpoint<DeleteUrlCommand, bool>
{
  private readonly IMediator _mediator;

  public DeleteUrlEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/urls/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteUrlCommand req, CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

    if (!result)
    {
      await Send.NotFoundAsync(ct);
      return;
    }

    await Send.OkAsync(result, ct);
  }
}
