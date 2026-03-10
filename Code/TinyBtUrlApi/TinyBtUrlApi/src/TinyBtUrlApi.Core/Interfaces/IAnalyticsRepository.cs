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
  Task<List<UserActivityDto>> GetUserActivityAsync(int userId, CancellationToken ct);

}
