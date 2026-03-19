using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.ForgotPassword;

namespace TinyBtUrlApi.Web.Auth.Create;

public static class ForgotPasswordEndpoint
{
  public static void MapForgotPasswordEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/auth/forgot-password",
        async (ForgotPasswordDto dto,
               ForgotPasswordHandler handler) =>
        {
          await handler.Handle(
                  new ForgotPasswordQuery(dto.Email));

          return Results.Ok(new
          {
            message = "If account exists, reset link sent."
          });
        })
        .WithTags("Auth")
        .AllowAnonymous();
  }
}
