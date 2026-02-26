using FastEndpoints;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Login;

namespace TinyBtUrlApi.Web.Authentication.Create;

using FastEndpoints;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Login;

public class LoginEndpoint : Endpoint<LoginDto, object>
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
    var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

    if (string.IsNullOrWhiteSpace(ip))
    {
      ip = "Unknown";
    }

    var result = await _handler.Handle(new LoginQuery(req, ip));

    var hasToken = false;

    if (result != null)
    {
      var prop = result.GetType().GetProperty("accessToken");
      hasToken = prop != null;
    }

    if (!hasToken)
    {
      HttpContext.Response.StatusCode = 401;
    }

    Response = result!;
  }
}
