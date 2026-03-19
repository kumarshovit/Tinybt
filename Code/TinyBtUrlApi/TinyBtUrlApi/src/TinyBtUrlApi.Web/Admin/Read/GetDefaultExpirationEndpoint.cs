using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Admin.GetDefaultExpiration;
namespace TinyBtUrlApi.Web.Admin.Read;

public class GetDefaultExpirationEndpoint
    : EndpointWithoutRequest
{
  private readonly IMediator _mediator;

  public GetDefaultExpirationEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/admin/settings/expiration");
    Roles("Admin"); // 🔒 Admin only
    Description(x => x.WithTags("Admin"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await _mediator.Send(
        new GetDefaultExpirationQuery(), ct);

    await Send.OkAsync(new
    {
      defaultExpirationDays = result
    }, ct);
  }
}
