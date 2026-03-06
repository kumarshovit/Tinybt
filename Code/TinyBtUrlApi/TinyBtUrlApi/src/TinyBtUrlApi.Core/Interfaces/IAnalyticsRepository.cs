using System;
using System.Collections.Generic;
using System.Text;

using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IAnalyticsRepository
{
  Task<int> GetTotalClicksAsync(DateTime from, DateTime to, CancellationToken ct);
  Task<int> GetUniqueVisitorsAsync(DateTime from, DateTime to, CancellationToken ct);
  Task<int> GetTotalUrlsAsync(CancellationToken ct);
  Task<int> GetTotalTagsAsync(CancellationToken ct);

  Task<int> GetActiveLinksAsync(CancellationToken ct);
  Task<int> GetExpiredLinksAsync(CancellationToken ct);
  Task<DashboardOverviewDto> GetUserDashboardAsync(
      int userId,
      DateTime from,
      DateTime to,
      CancellationToken ct);

  Task<List<AnalyticsItemDto>> GetAnalyticsByFieldAsync(
        int userId,
        DateTime from,
        DateTime to,
        string field);
  Task<List<AnalyticsItemDto>> GetClicksOverTimeByUserAsync(
    int userId,
    DateTime from,
    DateTime to,
    CancellationToken ct);
  Task<List<AnalyticsItemDto>> GetPopularLinksByUserAsync(
    int userId,
    DateTime from,
    DateTime to,
    CancellationToken ct);
  Task<List<AnalyticsItemDto>> GetDeviceLanguageByUserAsync(
    int userId,
    DateTime from,
    DateTime to,
    CancellationToken ct);
}
