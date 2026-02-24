using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.SearchByTag;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class SearchByTagEndpoint
    : EndpointWithoutRequest<List<UrlMapping>>
{
  private readonly IMediator _mediator;

  public SearchByTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/urls/{search}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var tag = Query<string>("tag");

    if (string.IsNullOrWhiteSpace(tag))
    {
      AddError("Tag query parameter is required.");
      await Send.ErrorsAsync();
      return;
    }

    var result = await _mediator.Send(
        new SearchByTagQuery(tag),
        ct);

    await Send.OkAsync(result);
  }
}
