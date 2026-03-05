using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetAnalyticsHandler
    : IRequestHandler<GetAnalyticsQuery, List<AnalyticsItemDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetAnalyticsHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<AnalyticsItemDto>> Handle(
      GetAnalyticsQuery request,
      CancellationToken ct)
  {
    return await _repo.GetAnalyticsByFieldAsync(
        request.UserId,
        request.From,
        request.To,
        request.Type);
  }
}
