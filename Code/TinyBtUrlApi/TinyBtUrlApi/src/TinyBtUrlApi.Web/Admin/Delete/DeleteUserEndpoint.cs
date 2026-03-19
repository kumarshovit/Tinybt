using FastEndpoints;
using TinyBtUrlApi.UseCases.Admin.DeleteUser;

namespace TinyBtUrlApi.Web.Admin.Delete;

public class ToggleUserStatusEndpoint
    : EndpointWithoutRequest<object>
{
  private readonly ToggleUserStatusHandler _handler;

  public ToggleUserStatusEndpoint(ToggleUserStatusHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Put("/api/admin/toggle-user/{UserId}");
    Roles("Admin");
    Description(x => x.WithTags("Admin"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var userId = Route<int>("UserId");

    var result = await _handler.Handle(
        new ToggleUserStatusCommand(userId)   // ✅ FIXED HERE
    );

    Response = result;
  }
}
