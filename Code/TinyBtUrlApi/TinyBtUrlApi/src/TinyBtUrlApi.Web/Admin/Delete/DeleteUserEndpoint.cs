using FastEndpoints;
using TinyBtUrlApi.UseCases.Admin.DeleteUser;

namespace TinyBtUrlApi.Web.Admin.Delete;

public class DeleteUserEndpoint
    : Endpoint<DeleteUserCommand, object>
{
  private readonly DeleteUserHandler _handler;

  public DeleteUserEndpoint(DeleteUserHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Delete("/api/admin/delete-user/{UserId}");
    Roles("Admin");
  }

  public override async Task HandleAsync(
      DeleteUserCommand req,
      CancellationToken ct)
  {
    var result = await _handler.Handle(req);

    Response = result;  // ✅ SAME STYLE
  }
}
