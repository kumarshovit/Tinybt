using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

public sealed class AnalyticsRepository(AppDbContext context)
    : IAnalyticsRepository
{
  public async Task<int> GetTotalClicksAsync(
      DateTime from, DateTime to, CancellationToken ct)
  {
    return await context.ClickLogs
        .AsNoTracking()
        .Where(x => x.ClickedAt >= from && x.ClickedAt <= to)
        .CountAsync(ct);
  }

  public async Task<int> GetUniqueVisitorsAsync(
      DateTime from, DateTime to, CancellationToken ct)
  {
    return await context.ClickLogs
        .AsNoTracking()
        .Where(x => x.ClickedAt >= from && x.ClickedAt <= to)
        .Select(x => x.VisitorId)
        .Distinct()
        .CountAsync(ct);
  }

  public async Task<int> GetTotalUrlsAsync(CancellationToken ct)
  {
    return await context.UrlMappings
        .AsNoTracking()
        .CountAsync(ct);
  }

  public async Task<int> GetTotalTagsAsync(CancellationToken ct)
  {
    return await context.Tags
        .AsNoTracking()
        .CountAsync(ct);
  }
}
