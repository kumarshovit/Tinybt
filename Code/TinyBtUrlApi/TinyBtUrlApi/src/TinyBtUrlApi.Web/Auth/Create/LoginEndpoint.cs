using FastEndpoints;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Login;

namespace TinyBtUrlApi.Web.Authentication.Create;

public class LoginEndpoint : Endpoint<LoginDto,object>
{
  private readonly LoginHandler _handler;

  public LoginEndpoint(LoginHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Post("/api/auth/login");
    AllowAnonymous();
  }

  public override async Task HandleAsync(LoginDto req, CancellationToken ct)
  {
    var result = await _handler.Handle(new LoginQuery(req));

    Response = result;   // ✅ SAME AS REGISTER
  }
}
