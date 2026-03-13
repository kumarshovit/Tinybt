using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetClicksHeatmapHandler : IRequestHandler<GetClicksHeatmapQuery, List<HeatmapDto>>
{
  private readonly IAnalyticsRepository repo;

  public GetClicksHeatmapHandler(IAnalyticsRepository repo)
  {
    this.repo = repo;
  }

  public async ValueTask<List<HeatmapDto>> Handle(
      GetClicksHeatmapQuery request,
      CancellationToken ct)
  {
    return await repo.GetClicksHeatmapAsync(
        request.UserId,
        request.Start,
        request.End,
        ct);
  }
}
