using Microsoft.AspNetCore.Authorization;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Logout;

namespace TinyBtUrlApi.Web.Auth.Create;

public static class LogoutEndpoint
{
  public static void MapLogoutEndpoint(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/auth/logout",
    [Authorize] async (
        RefreshTokenDto req,
        HttpContext context,
        LogoutHandler handler) =>
    {
      if (req == null || string.IsNullOrEmpty(req.RefreshToken))
        return Results.BadRequest("Refresh token required.");

      var accessToken = context.Request.Headers["Authorization"]
          .ToString()
          .Replace("Bearer ", "");

      if (string.IsNullOrEmpty(accessToken))
        return Results.BadRequest("Access token missing.");

      await handler.HandleAsync(
          new LogoutQuery(accessToken, req.RefreshToken)
      );

      return Results.Ok(new { message = "Logged out successfully" });
    })
    .WithTags("Api");
  }
}
