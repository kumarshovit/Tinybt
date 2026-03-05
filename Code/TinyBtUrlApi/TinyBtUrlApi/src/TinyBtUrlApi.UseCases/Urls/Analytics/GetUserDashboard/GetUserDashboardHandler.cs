using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetUserDashboardHandler
    : IRequestHandler<GetUserDashboardQuery, DashboardOverviewDto>
{
  private readonly IAnalyticsRepository _repo;

  public GetUserDashboardHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<DashboardOverviewDto> Handle(
      GetUserDashboardQuery request,
      CancellationToken ct)
  {
    return await _repo.GetUserDashboardAsync(
        request.UserId,
        request.From,
        request.To,
        ct);
  }
}
