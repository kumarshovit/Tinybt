using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.RenameTag;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class RenameTagEndpoint
    : Endpoint<RenameTagRequest>
{
  private readonly IMediator _mediator;

  public RenameTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/urls/{urlId}/tags/{oldTag}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(RenameTagRequest req, CancellationToken ct)
  {
    var urlId = Route<int>("urlId");
    var oldTag = Route<string>("oldTag");

    

    if (string.IsNullOrWhiteSpace(oldTag))
    {
      AddError("Old tag is required.");
      await Send.ErrorsAsync();
      return;
    }

    if (string.IsNullOrWhiteSpace(req.NewTag))
    {
      AddError("New tag is required.");
      await Send.ErrorsAsync();
      return;
    }

    var command = new RenameTagCommand(urlId, oldTag, req.NewTag);

    await _mediator.Send(command, ct);

    await Send.OkAsync("this tag renamed sucessfully ");
  }
}
