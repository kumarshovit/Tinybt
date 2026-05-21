using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.AddTags;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class AddTagsEndpoint
    : Endpoint<AddTagsRequest>
{
  private readonly IMediator _mediator;

  public AddTagsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/urls/{urlId}/tags");
    Roles("User", "Admin");
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(AddTagsRequest req, CancellationToken ct)
  {
    var idStr = Route<string>("urlId");

    if (!int.TryParse(idStr, out var urlId))
    {
      AddError("Invalid UrlId.");
      await Send.ErrorsAsync();
      return;
    }

    var command = new AddTagsCommand(urlId, req.Tags);

    await _mediator.Send(command, ct);

    await Send.OkAsync("Add tags successfully");
  }
}
