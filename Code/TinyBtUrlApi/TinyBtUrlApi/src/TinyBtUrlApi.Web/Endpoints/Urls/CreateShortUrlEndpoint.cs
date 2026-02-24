using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class CreateShortUrlEndpoint
    : Endpoint<CreateShortUrlCommand, CreateShortUrlResult>
{
  private readonly IMediator _mediator;

  public CreateShortUrlEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/urls");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
      CreateShortUrlCommand req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

    await Send.OkAsync(result, ct);
  }
}
