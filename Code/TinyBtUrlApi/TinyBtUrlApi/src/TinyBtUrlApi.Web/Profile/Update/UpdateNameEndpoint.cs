using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Account.Profile;

namespace TinyBtUrlApi.Web.Profile.Update;

public static class UpdateNameEndpoint
{
  public static void MapUpdateNameEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapPut("/api/profile/name",
    async (UpdateProfileDto dto,
           HttpContext context,
           [FromServices] UpdateProfileHandler handler) =>
    {
      var userId = int.Parse(
          context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

      var success = await handler.Handle(
          new UpdateProfileCommand(userId, dto.FullName));

      if (!success)
        return Results.NotFound();

      return Results.Ok(new
      {
        message = "Name updated successfully."
      });
    })
    .WithTags("Profile")
    .RequireAuthorization();
  }
}
