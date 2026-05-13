using FastEndpoints;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Register;

namespace TinyBtUrlApi.Web.Authentication.Create;

public class RegisterEndpoint : Endpoint<RegisterDto, RegisterResponse>
{
  private readonly RegisterHandler _handler;

  public RegisterEndpoint(RegisterHandler handler)
  {
    _handler = handler;
  }

  public override void Configure()
  {
    Post("/api/auth/register");
    AllowAnonymous();
    Description(x => x.WithTags("Auth"));
  }

  public override async Task HandleAsync(RegisterDto req, CancellationToken ct)
  {
    var result = await _handler.Handle(new RegisterQuery(req));

    if (!result.Success)
    {
      await Send.ResultAsync(
          Results.BadRequest(result)
      );

      return;
    }

    await Send.ResultAsync(
        Results.Ok(result)
    );
  }
}
