using System.Security.Claims;
using TinyBtUrlApi.UseCases.Account.Profile;

namespace TinyBtUrlApi.Web.Profile.Read;

public static class GetProfileEndpoint
{
  public static void MapGetProfileEndpoint(
      this IEndpointRouteBuilder app)
  {
    app.MapGet("/api/profile",
        async (HttpContext context,
               GetProfileHandler handler) =>
        {
          var userId = int.Parse(
              context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

          var result = await handler.Handle(
              new GetProfileQuery(userId));

          if (result == null)
            return Results.NotFound();

          return Results.Ok(result);
        })
        .WithTags("Profile")
        .RequireAuthorization();
  }
}
