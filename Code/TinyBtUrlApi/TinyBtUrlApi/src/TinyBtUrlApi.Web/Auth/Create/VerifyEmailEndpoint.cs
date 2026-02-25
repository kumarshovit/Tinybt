using Microsoft.AspNetCore.Http.HttpResults;
using TinyBtUrlApi.UseCases.Account.VerifyEmail;

namespace TinyBtUrlApi.Web.Auth;

public class VerifyEmail(VerifyEmailHandler handler)
  : EndpointWithoutRequest<Results<Ok<string>, BadRequest<string>>>
{
  private readonly VerifyEmailHandler _handler = handler;

  public override void Configure()
  {
    Get("/api/auth/verify-email");
    AllowAnonymous();
  }

  public override async Task<Results<Ok<string>, BadRequest<string>>>
    ExecuteAsync(CancellationToken ct)
  {
    var token = Query<string>("token");

    if (string.IsNullOrEmpty(token))
      return TypedResults.BadRequest("Token is required.");

    var result = await _handler.Handle(new VerifyEmailQuery(token));

    if (result.Contains("success"))
      return TypedResults.Ok(result);

    return TypedResults.BadRequest(result);
  }
}
