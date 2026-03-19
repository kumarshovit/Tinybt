using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Admin.UpdateDefaultExpiration;
using TinyBtUrlApi.UseCases.Admin.UpdateDefaultExpiration.cs;

namespace TinyBtUrlApi.Web.Admin.Update;

public class UpdateDefaultExpirationEndpoint
    : Endpoint<UpdateDefaultExpirationCommand>
{
  private readonly IMediator _mediator;

  public UpdateDefaultExpirationEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Put("/api/admin/settings/expiration");
    Roles("Admin"); // 🔒 Admin only
    Description(x => x.WithTags("Admin"));
  }

  public override async Task HandleAsync(
      UpdateDefaultExpirationCommand req,
      CancellationToken ct)
  {
    await _mediator.Send(req, ct);

    await Send.OkAsync(new
    {
      message = "Default expiration updated successfully."
    }, ct);
  }
}
