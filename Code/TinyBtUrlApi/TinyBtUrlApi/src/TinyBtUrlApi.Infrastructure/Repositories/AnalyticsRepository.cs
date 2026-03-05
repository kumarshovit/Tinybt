using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

public sealed class AnalyticsRepository : IAnalyticsRepository
{
  private readonly AppDbContext context;

  public AnalyticsRepository(AppDbContext context)
  {
    this.context = context;
  }

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

  public async Task<int> GetActiveLinksAsync(CancellationToken ct)
  {
    var now = DateTime.UtcNow;

    return await context.UrlMappings
        .AsNoTracking()
        .Where(x =>
            !x.IsDeleted &&
            (x.ExpirationDate == null || x.ExpirationDate > now))
        .CountAsync(ct);
  }

  public async Task<int> GetExpiredLinksAsync(CancellationToken ct)
  {
    var now = DateTime.UtcNow;

    return await context.UrlMappings
        .AsNoTracking()
        .Where(x =>
            !x.IsDeleted &&
            x.ExpirationDate != null &&
            x.ExpirationDate <= now)
        .CountAsync(ct);
  }

  public async Task<DashboardOverviewDto> GetUserDashboardAsync(
     int userId,
     DateTime startDate,
     DateTime endDate,
     CancellationToken ct)
  {

    var urls = context.UrlMappings
.Where(x => x.UserId == userId && x.IsDeleted == false);
    var totalUrls = await urls.CountAsync(ct);
    var activeLinks = await urls
        .Where(x => x.ExpirationDate == null ||
                    x.ExpirationDate.Value > DateTime.UtcNow)
        .CountAsync(ct);

    var expiredLinks = await urls
        .Where(x => x.ExpirationDate != null &&
                    x.ExpirationDate.Value <= DateTime.UtcNow)
        .CountAsync(ct);

    var clickLogs =
       from c in context.ClickLogs
       join u in context.UrlMappings
           on c.ShortCode equals u.ShortCode
       where u.UserId == userId &&
             c.ClickedAt >= startDate &&
             c.ClickedAt <= endDate
       select c;

    var totalClicks = await clickLogs.CountAsync(ct);

    var uniqueVisitors = await clickLogs
        .Select(x => x.VisitorId)
        .Distinct()
        .CountAsync(ct);

    var totalTags = await context.UrlTags
        .Where(x => x.UrlMapping.UserId == userId)
        .CountAsync(ct);

    return new DashboardOverviewDto
    {
      TotalClicks = totalClicks,
      UniqueVisitors = uniqueVisitors,
      TotalUrls = totalUrls,
      TotalTags = totalTags,
      ActiveLinks = activeLinks,
      ExpiredLinks = expiredLinks,
      From = startDate,
      To = endDate
    };
  }
  public async Task<List<AnalyticsItemDto>> GetAnalyticsByFieldAsync(
    int userId,
    DateTime start,
    DateTime end,
    string field)
  {
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId
        && c.ClickedAt >= start
        && c.ClickedAt <= end
        select c;

    return field switch
    {
      "referrer" => await query
          .GroupBy(x => x.Referrer)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Direct",
            Count = g.Count()
          }).ToListAsync(),

      "country" => await query
          .GroupBy(x => x.Country)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Unknown",
            Count = g.Count()
          }).ToListAsync(),

      "device" => await query
          .GroupBy(x => x.DeviceType)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Unknown",
            Count = g.Count()
          }).ToListAsync(),

      "browser" => await query
          .GroupBy(x => x.Browser)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Unknown",
            Count = g.Count()
          }).ToListAsync(),

      "os" => await query
          .GroupBy(x => x.OS)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Unknown",
            Count = g.Count()
          }).ToListAsync(),

      "language" => await query
          .GroupBy(x => x.DeviceLanguage)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key ?? "Unknown",
            Count = g.Count()
          }).ToListAsync(),

      _ => new List<AnalyticsItemDto>()
    };
  }

  public async Task<List<AnalyticsItemDto>> GetClicksOverTimeByUserAsync(
    int userId,
    DateTime start,
    DateTime end,
    CancellationToken ct)
  {
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId
        && c.ClickedAt >= start
        && c.ClickedAt <= end
        select c;

    var data = await query
        .GroupBy(x => x.ClickedAt.Date)
        .Select(g => new
        {
          Date = g.Key,
          Count = g.Count()
        })
        .OrderBy(x => x.Date)
        .ToListAsync(ct);

    return data.Select(x => new AnalyticsItemDto
    {
      Label = x.Date.ToString("yyyy-MM-dd"),
      Count = x.Count
    }).ToList();
  }
}
