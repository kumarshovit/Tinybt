using FastEndpoints;
using Mediator;
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
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var id = Route<int>("id");   // read from route

    var result = await _mediator.Send(new DeleteUrlCommand(id), ct);

    if (!result)
    {
      await Send.NotFoundAsync(ct);
      return;
    }

    await Send.OkAsync(result, ct);
  }
}
