using FastEndpoints;
using TinyBtUrlApi.UseCases.Admin.UpdateUserRole;

namespace TinyBtUrlApi.Web.Admin.Update;

public class UpdateUserRoleEndpoint
    : Endpoint<UpdateUserRoleCommand, object>
{
  private readonly UpdateUserRoleHandler _handler;

  public UpdateUserRoleEndpoint(UpdateUserRoleHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Put("/api/admin/update-role");
    Roles("Admin");
    Description(x => x.WithTags("Admin"));
  }

  public override async Task HandleAsync(
      UpdateUserRoleCommand req,
      CancellationToken ct)
  {
    var result = await _handler.Handle(req);

    Response = result;  // ✅ SAME STYLE
  }
}
