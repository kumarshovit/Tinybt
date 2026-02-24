using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.UpdateAlias;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class UpdateAliasEndpoint
    : Endpoint<UpdateAliasRequest, string>
{
  private readonly IMediator _mediator;

  public UpdateAliasEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/urls/{id}/alias");
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateAliasRequest req, CancellationToken ct)
  {
    var id = Route<int>("id");

    var command = new UpdateAliasCommand(id, req.NewAlias);

    var result = await _mediator.Send(command, ct);

    if (result is null)
    {
      AddError("Alias already exists or URL not found.");
      await Send.ErrorsAsync();
      return;
    }

    await Send.OkAsync(result, ct);
  }
}
