using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.UpdateTags;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class UpdateTagsEndpoint
    : Endpoint<AddTagsRequest>
{
  private readonly IMediator _mediator;

  public UpdateTagsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/urls/{urlId}/tags");
    AllowAnonymous();
  }

  public override async Task HandleAsync(AddTagsRequest req, CancellationToken ct)
  {
    var urlId= Route<int>("urlId");

   

    var command = new UpdateTagsCommand(urlId, req.Tags);

    await _mediator.Send(command, ct);

    await Send.OkAsync("Success");
  }
}
