using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Profile;

namespace TinyBtUrlApi.Web.Profile.Update;

public static class ChangePasswordEndpoint
{
  public static void MapChangePasswordEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapPut("/api/profile/password",
    async (ChangePasswordDto dto,
           HttpContext context,
           [FromServices] ChangePasswordHandler handler) =>
    {
      var userId = int.Parse(
          context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

      var result = await handler.Handle(
          new ChangePasswordCommand(
              userId,
              dto.CurrentPassword,
              dto.NewPassword));

      if (!result.Success)
        return Results.BadRequest(new
        {
          message = result.Message
        });

      return Results.Ok(new
      {
        message = result.Message
      });
    })
    .WithTags("Profile")
    .RequireAuthorization();
  }
}
