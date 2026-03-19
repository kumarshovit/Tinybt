using FastEndpoints;
using TinyBtUrlApi.UseCases.Admin.AdminAnalysis;

namespace TinyBtUrlApi.Web.Admin.AdminAnalysis;

public class GetUsersByBrowserEndpoint
    : Endpoint<GetUsersByBrowserRequest>
{
  private readonly IMediator _mediator;

  public GetUsersByBrowserEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/admin/analytics/users-by-browser");

    AllowAnonymous();
    // 🔥 Change later:
    // Roles("Admin");
  }

  public override async Task HandleAsync(
      GetUsersByBrowserRequest req,
      CancellationToken ct)
  {
    // 🔥 DEFAULT LAST 7 DAYS
    var endDate = req.EndDate ?? DateTime.UtcNow;
    var startDate = req.StartDate ?? endDate.AddDays(-7);

    // 🔥 SAFETY CHECK
    //if (startDate > endDate)
    //{
    //  await SendBadRequestAsync(ct);
    //  return;
    //}

    var result = await _mediator.Send(new GetUsersByBrowserQuery
    {
      StartDate = startDate,
      EndDate = endDate
    });

    await Send.OkAsync(result, ct);
  }
}
