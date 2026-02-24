using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.GetAllUrls;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class GetAllUrlsEndpoint
    : EndpointWithoutRequest<List<UrlDto>>
{
  private readonly IMediator _mediator;

  public GetAllUrlsEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/urls");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetAllUrlsQuery(), ct);

    await Send.OkAsync(result, ct);
  }
}
