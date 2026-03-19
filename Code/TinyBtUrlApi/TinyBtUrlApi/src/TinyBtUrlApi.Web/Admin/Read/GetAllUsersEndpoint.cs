using FastEndpoints;
using TinyBtUrlApi.UseCases.Admin.GetAllUsers;

namespace TinyBtUrlApi.Web.Admin.Read;

public class GetAllUsersEndpoint
    : EndpointWithoutRequest<object>
{
  private readonly GetAllUsersHandler _handler;

  public GetAllUsersEndpoint(GetAllUsersHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Get("/api/admin/all-users");
    Roles("Admin");   // 🔥 role restriction
    Description(x => x.WithTags("Admin"));
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await _handler.Handle(new GetAllUsersQuery());

    Response = result;   // ✅ SAME PATTERN AS LOGIN
  }
}
