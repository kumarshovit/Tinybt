using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.RemoveTag;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class RemoveTagEndpoint : EndpointWithoutRequest
{
  private readonly IMediator _mediator;

  public RemoveTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Delete("/api/urls/{urlId}/tags/{tag}");
    AllowAnonymous();
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var urlId = Route<int>("urlId");
    var tag = Route<string>("tag");

    if (string.IsNullOrWhiteSpace(tag))
    {
      AddError("Tag is required.");
      await Send.ErrorsAsync();
      return;
    }

    var command = new RemoveTagCommand(urlId, tag);

    await _mediator.Send(command, ct);

    await Send.OkAsync("This tag removed ");
  }
}
