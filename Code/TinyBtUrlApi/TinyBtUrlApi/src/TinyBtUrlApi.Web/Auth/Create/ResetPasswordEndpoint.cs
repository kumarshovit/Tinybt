using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.ResetPassword;

namespace TinyBtUrlApi.Web.Auth.Create;

public static class ResetPasswordEndpoint
{
  public static void MapResetPasswordEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/auth/reset-password",
        async (ResetPasswordDto dto,
               ResetPasswordHandler handler) =>
        {
          var result = await handler.Handle(
                  new ResetPasswordQuery(
                      dto.Email,
                      dto.Token,
                      dto.NewPassword));

          if (!result)
            return Results.BadRequest(
                    "Invalid or expired token.");

          return Results.Ok(new
          {
            message = "Password reset successful."
          });
        })
        .WithTags("Api")
        .AllowAnonymous();
  }
}
