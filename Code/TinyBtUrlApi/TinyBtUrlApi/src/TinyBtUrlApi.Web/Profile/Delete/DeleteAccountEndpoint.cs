using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TinyBtUrlApi.UseCases.Account.Delete;

namespace TinyBtUrlApi.Web.Profile.Delete;

public static class DeleteAccountEndpoint
{
  public static void MapDeleteAccountEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapDelete("/api/profile",
     async (HttpContext context,
            [FromServices] DeleteAccountHandler handler) =>
     {
       var userId = int.Parse(
          context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

       var success = await handler.Handle(
          new DeleteAccountCommand(userId));

       if (!success)
         return Results.NotFound();

       return Results.Ok(new
       {
         message = "Account deleted successfully."
       });
     })
     .WithTags("Profile")
     .RequireAuthorization();
  }
}
