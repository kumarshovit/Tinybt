//using FastEndpoints;
//using Mediator;
//using System.Security.Claims;
//using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

//namespace TinyBtUrlApi.Web.Endpoints.Analytics;

//public class GetClicksHeatmapEndpoint : EndpointWithoutRequest
//{
//  private readonly IMediator mediator;

//  public GetClicksHeatmapEndpoint(IMediator mediator)
//  {
//    this.mediator = mediator;
//  }

//  public override void Configure()
//  {
//    Get("/analytics/heatmap");

//    // Require logged-in user
//    AuthSchemes("Bearer");
//  }

//  public override async Task HandleAsync(CancellationToken ct)
//  {
//    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

//    if (userIdClaim == null)
//    {
//      await Send.UnauthorizedAsync();
//      return;
//    }

//    var userId = int.Parse(userIdClaim.Value);

//    var start = DateTime.UtcNow.AddDays(-30);
//    var end = DateTime.UtcNow;

//    var data = await mediator.Send(
//        new GetClicksHeatmapQuery(userId, start, end),
//        ct);

//    await Send.OkAsync(data);
//  }
//}


//using FastEndpoints;
//using Mediator;
//using System.Security.Claims;
//using TinyBtUrlApi.Web.Endpoints.Analytics;
//using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

//namespace TinyBtUrlApi.Web.Endpoints.Analytics;

//public class GetClicksHeatmapEndpoint
//    : Endpoint<GetAnalyticsRequest, object>
//{
//  private readonly IMediator mediator;

//  public GetClicksHeatmapEndpoint(IMediator mediator)
//  {
//    this.mediator = mediator;
//  }

//  public override void Configure()
//  {
//    Post("/analytics/heatmap"); // ✅ MUST BE POST
//    Roles("User", "Admin");
//  }

// public override async Task HandleAsync(GetAnalyticsRequest req, CancellationToken ct)
//{
//    var claim = User.FindFirst(ClaimTypes.NameIdentifier);

//    if (claim == null)
//    {
//        await Send.UnauthorizedAsync(ct);
//        return;
//    }

//    int loggedInUserId = int.Parse(claim.Value);

//    int userId;

//    // 🔥 ROLE-BASED CONTROL
//    if (User.IsInRole("Admin"))
//    {
//        userId = req.UserId ?? loggedInUserId;
//    }
//    else
//    {
//        userId = loggedInUserId;
//    }

//    // 🔥 Default dates (important)
//    var from = req.From == default
//        ? DateTime.UtcNow.AddDays(-7)
//        : req.From;

//    var to = req.To == default
//        ? DateTime.UtcNow
//        : req.To;

//    var data = await mediator.Send(
//        new GetClicksHeatmapQuery(userId, from, to),
//        ct);

//    await Send.OkAsync(data);
//}
//}

using System.Security.Claims;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetClicksHeatmapEndpoint
    : Endpoint<GetAnalyticsRequest, List<HeatmapDto>>
{
  private readonly IMediator mediator;

  public GetClicksHeatmapEndpoint(IMediator mediator)
  {
    this.mediator = mediator;
  }

  public override void Configure()
  {
    Post("/analytics/heatmap");
    Roles("User", "Admin");
  }

  public override async Task HandleAsync(GetAnalyticsRequest req, CancellationToken ct)
  {
    var claim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim == null)
    {
      await Send.UnauthorizedAsync(ct);
      return;
    }

    int loggedInUserId = int.Parse(claim.Value);

    int finalUserId;

    // ✅ FIXED ROLE LOGIC
    if (User.IsInRole("Admin"))
    {
      // Admin → own data by default OR selected user
      finalUserId = req.UserId.HasValue && req.UserId > 0
          ? req.UserId.Value
          : loggedInUserId;
    }
    else
    {
      // 🔥 USER → ALWAYS OWN DATA (ignore req.UserId completely)
      finalUserId = loggedInUserId;
    }

    // ✅ DEFAULT DATE RANGE
    var from = req.From == default
        ? DateTime.UtcNow.AddDays(-7)
        : req.From;

    var to = req.To == default
        ? DateTime.UtcNow
        : req.To;

    var data = await mediator.Send(
        new GetClicksHeatmapQuery(finalUserId, from, to),
        ct);

    await Send.OkAsync(data);
  }
}
