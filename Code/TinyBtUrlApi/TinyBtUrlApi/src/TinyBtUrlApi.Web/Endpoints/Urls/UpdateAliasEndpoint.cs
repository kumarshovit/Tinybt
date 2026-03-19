using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Urls.UpdateAlias;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;
using TinyBtUrlApi.Web.Endpoints.Urls.Responses;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class UpdateAliasEndpoint
    : Endpoint<UpdateAliasRequest, UrlResponse>
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
    Description(x => x.WithTags("Url Management"));
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

    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

    var response = new UrlResponse
    {
      Id = result.Id,
      LongUrl = result.LongUrl,
      ShortCode = result.ShortCode,
      ShortUrl = $"{baseUrl}/{result.ShortCode}",
      ExpirationDate = result.ExpirationDate,
      ClickCount = result.ClickCount
    };

    await Send.OkAsync(response, ct);
  }
}
