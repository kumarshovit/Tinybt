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
            .GroupBy(x =>
                string.IsNullOrEmpty(x.Referrer)
                ? "Direct"
                : x.Referrer.Contains("google")
                    ? "Google"
                    : x.Referrer.Contains("linkedin")
                        ? "LinkedIn"
                        : x.Referrer.Contains("twitter")
                            ? "Twitter"
                            : "Other")
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

        "country" => await query
            .GroupBy(x =>
                string.IsNullOrEmpty(x.Country)
                ? "Unknown"
                : x.Country)
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

        "device" => await query
            .GroupBy(x =>
                string.IsNullOrEmpty(x.DeviceType)
                ? "Unknown"
                : x.DeviceType)
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

        "browser" => await query
            .GroupBy(x =>
                string.IsNullOrEmpty(x.Browser)
                ? "Unknown"
                : x.Browser)
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

        "os" => await query
            .GroupBy(x =>
                string.IsNullOrEmpty(x.OS)
                ? "Unknown"
                : x.OS)
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

        "language" => await query
            .GroupBy(x =>
                string.IsNullOrEmpty(x.DeviceLanguage)
                ? "Unknown"
                : x.DeviceLanguage)
            .Select(g => new AnalyticsItemDto
            {
                Label = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(),

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
  public async Task<List<AnalyticsItemDto>> GetPopularLinksByUserAsync(
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

    return await query
        .GroupBy(x => x.ShortCode)
        .Select(g => new AnalyticsItemDto
        {
          Label = g.Key,
          Count = g.Count()
        })
        .OrderByDescending(x => x.Count)
        .Take(10)
        .ToListAsync(ct);
  }

  public async Task<List<AnalyticsItemDto>> GetDeviceLanguageByUserAsync(
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

    return await query
        .GroupBy(x => x.DeviceLanguage)
        .Select(g => new AnalyticsItemDto
        {
          Label = g.Key ?? "Unknown",
          Count = g.Count()
        })
        .OrderByDescending(x => x.Count)
        .ToListAsync(ct);
  }
}
