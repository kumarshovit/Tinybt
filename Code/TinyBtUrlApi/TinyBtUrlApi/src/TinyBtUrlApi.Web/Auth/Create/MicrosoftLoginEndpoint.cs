using Microsoft.AspNetCore.Mvc;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Login;

namespace TinyBtUrlApi.Web.Auth.Create;

public static class MicrosoftLoginEndpoint
{
  public static void MapMicrosoftLoginEndpoint(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/auth/microsoft-login",
    async (MicrosoftLoginDto dto,
           MicrosoftLoginHandler handler) =>
    {
      if (string.IsNullOrEmpty(dto.Token))
        return Results.BadRequest("Token is required.");

      var result = await handler.Handle(
              new MicrosoftLoginQuery(dto.Token));

      if (result == null)
      {
        return Results.BadRequest(new
        {
          message = "Your account has been deleted or login failed."
        });
      }

      var (token, refreshToken, expires) = result.Value;

      return Results.Ok(new
      {
        accessToken = token,
        refreshToken = refreshToken,
        expires
      });
    })
    .WithTags("Auth")
    .AllowAnonymous();
  }
}

