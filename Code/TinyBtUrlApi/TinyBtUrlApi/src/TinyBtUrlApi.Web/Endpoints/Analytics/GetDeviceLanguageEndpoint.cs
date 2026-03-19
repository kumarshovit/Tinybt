using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;
using TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetDeviceLanguageEndpoint
    : Endpoint<DeviceLanguageRequest, List<AnalyticsItemDto>>
{
  private readonly IMediator _mediator;

  public GetDeviceLanguageEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/analytics/user/device-language");
    Roles("User", "Admin"); // 👈 UPDATED
    Description(x => x.WithTags("Analytics (User specific)"));
  }

  public override async Task HandleAsync(
    DeviceLanguageRequest req,
    CancellationToken ct)
  {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (userIdClaim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int loggedInUserId = int.Parse(userIdClaim.Value);

    // ✅ Check if admin
    bool isAdmin = User.IsInRole("Admin");

    // 🔥 MAIN FIX → use selected user if admin
    int finalUserId = isAdmin && req.UserId.HasValue
        ? req.UserId.Value
        : loggedInUserId;

    var result = await _mediator.Send(
        new GetDeviceLanguageByUserQuery(
            finalUserId,   // ✅ FIXED
            req.From,
            req.To,
            req.Link,
            req.Tag
        ),
        ct);

    await Send.OkAsync(result);
  }
}
