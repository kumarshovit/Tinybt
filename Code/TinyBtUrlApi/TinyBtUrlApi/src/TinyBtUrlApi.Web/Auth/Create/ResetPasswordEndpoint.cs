using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.ResetPassword;

namespace TinyBtUrlApi.Web.Auth.Create;

public static class ResetPasswordEndpoint
{
  public static void MapResetPasswordEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/auth/reset-password",
        () => Results.BadRequest(new
        {
          message = "Password reset is not available. Please sign in with Google."
        }))
        .WithTags("Auth")
        .AllowAnonymous();
  }
}
