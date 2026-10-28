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
    Description(x => x.WithTags("Auth"));
  }

  public override async Task HandleAsync(LoginDto req, CancellationToken ct)
  {
    await Send.ResultAsync(
      Results.BadRequest(new
      {
        message = "Direct password login is disabled. Please sign in with Google."
      })
    );
  }
}
