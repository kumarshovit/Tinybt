using TinyBtUrlApi.Core.DTOs;
using Microsoft.EntityFrameworkCore;
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

    var endInclusive = endDate.AddDays(1);

    var clickLogs =
       from c in context.ClickLogs
       join u in context.UrlMappings
           on c.ShortCode equals u.ShortCode
       where u.UserId == userId &&
             c.ClickedAt >= startDate &&
             c.ClickedAt < endInclusive
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
    string field,
    string? link,
    string? tag)
  {
    var endInclusive = end.AddDays(1);
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId &&
       c.ClickedAt >= start &&
       c.ClickedAt < endInclusive
        select new { c, u };

    // LINK FILTER
    if (!string.IsNullOrEmpty(link))
    {
      query = query.Where(x => x.u.ShortCode == link);
    }

    // TAG FILTER
    if (!string.IsNullOrEmpty(tag))
    {
      query =
          from q in query
          join ut in context.UrlTags
          on q.u.Id equals ut.UrlMappingId
          join t in context.Tags
          on ut.TagId equals t.Id
          where t.Name == tag
          select q;
    }

    return field switch
    {
      "referrer" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.Referrer)
                  ? "Direct"
                  : x.c.Referrer.Trim())
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key,
            Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToListAsync(),

      "country" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.Country)
                  ? "Unknown"
                  : x.c.Country)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key,
            Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToListAsync(),

      "device" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.DeviceType)
                  ? "Unknown"
                  : x.c.DeviceType)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key,
            Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToListAsync(),

      "browser" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.Browser)
                  ? "Unknown"
                  : x.c.Browser)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key,
            Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToListAsync(),

      "os" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.OS)
                  ? "Unknown"
                  : x.c.OS)
          .Select(g => new AnalyticsItemDto
          {
            Label = g.Key,
            Count = g.Count()
          })
          .OrderByDescending(x => x.Count)
          .ToListAsync(),

      "language" => await query
          .GroupBy(x =>
              string.IsNullOrEmpty(x.c.DeviceLanguage)
                  ? "Unknown"
                  : x.c.DeviceLanguage)
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
     string? link,
     string? tag,
     CancellationToken ct)
  {
    var endInclusive = end.AddDays(1);
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId
        && c.ClickedAt >= start
        && c.ClickedAt < endInclusive
        select new { c, u };

    if (!string.IsNullOrEmpty(link))
    {
      query = query.Where(x => x.u.ShortCode == link);
    }

    if (!string.IsNullOrEmpty(tag))
    {
      query =
          from q in query
          join ut in context.UrlTags
          on q.u.Id equals ut.UrlMappingId
          join t in context.Tags
          on ut.TagId equals t.Id
          where t.Name == tag
          select q;
    }

    var data = await query
        .GroupBy(x => x.c.ClickedAt.Date)
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
    string? link,
    string? tag,
    CancellationToken ct)
  {
    var endInclusive = end.AddDays(1);
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId
        && c.ClickedAt >= start
        && c.ClickedAt <= endInclusive
        select new { c, u };

    if (!string.IsNullOrEmpty(link))
    {
      query = query.Where(x => x.u.ShortCode == link);
    }

    if (!string.IsNullOrEmpty(tag))
    {
      query =
          from q in query
          join ut in context.UrlTags
          on q.u.Id equals ut.UrlMappingId
          join t in context.Tags
          on ut.TagId equals t.Id
          where t.Name == tag
          select q;
    }

    return await query
        .GroupBy(x => x.c.ShortCode)
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
    string? link,
    string? tag,
    CancellationToken ct)
  {
    var endInclusive = end.AddDays(1);
    var query =
        from c in context.ClickLogs
        join u in context.UrlMappings
        on c.ShortCode equals u.ShortCode
        where u.UserId == userId
        && c.ClickedAt >= start
        && c.ClickedAt <= endInclusive
        select new { c, u };

    if (!string.IsNullOrEmpty(link))
    {
      query = query.Where(x => x.u.ShortCode == link);
    }

    if (!string.IsNullOrEmpty(tag))
    {
      query =
          from q in query
          join ut in context.UrlTags
          on q.u.Id equals ut.UrlMappingId
          join t in context.Tags
          on ut.TagId equals t.Id
          where t.Name == tag
          select q;
    }

    var data = await query
        .Select(x => x.c.DeviceLanguage)
        .ToListAsync(ct);

    return data
     .Where(x => !string.IsNullOrWhiteSpace(x))
     .Select(x => x!.Split(',')[0])
     .GroupBy(x => x)
     .Select(g => new AnalyticsItemDto
     {
       Label = g.Key,
       Count = g.Count()
     })
     .OrderByDescending(x => x.Count)
     .ToList();
  }


  public async Task<List<HeatmapDto>> GetClicksHeatmapAsync(
      int userId,
      DateTime start,
      DateTime end,
      CancellationToken ct)
  {
    // 1️⃣ user ke shortcodes
    var userShortCodes = await context.UrlMappings
        .Where(u => u.UserId == userId)
        .Select(u => u.ShortCode)
        .ToListAsync(ct);

    // 2️⃣ relevant click logs
    var clickLogs = await context.ClickLogs
        .Where(c =>
            c.ClickedAt >= start &&
            c.ClickedAt <= end &&
            userShortCodes.Contains(c.ShortCode))
        .ToListAsync(ct);

    // 3️⃣ memory me grouping
    return clickLogs
        .GroupBy(c => new
        {
          Day = (int)c.ClickedAt.DayOfWeek,
          Hour = c.ClickedAt.Hour
        })
        .Select(g => new HeatmapDto
        {
          Day = g.Key.Day,
          Hour = g.Key.Hour,
          Count = g.Count()
        }).ToList();
  }

  public async Task<List<UserActivityDto>> GetUserActivityAsync(int userId, CancellationToken ct)
  {
    var activities = new List<UserActivityDto>();

    var links = await context.UrlMappings
        .AsNoTracking()
        .Where(x => x.UserId == userId)
        .ToListAsync(ct);

    foreach (var link in links)
    {
      // Link created
      activities.Add(new UserActivityDto
      {
        ActivityType = "Link Created",
        ShortCode = link.ShortCode,
        LongUrl = link.LongUrl,
        ActivityTime = link.CreatedAt
      });

      // Link deleted
      if (link.DeletedAt != null)
      {
        activities.Add(new UserActivityDto
        {
          ActivityType = "Link Deleted",
          ShortCode = link.ShortCode,
          LongUrl = link.LongUrl,
          ActivityTime = link.DeletedAt.Value
        });
      }

      //// Clicks on this link
      //var clicks = await context.ClickLogs
      //    .AsNoTracking()
      //    .Where(c => c.ShortCode == link.ShortCode)
      //    .ToListAsync(ct);

      //foreach (var click in clicks)
      //{
      //  activities.Add(new UserActivityDto
      //  {
      //    ActivityType = "Link Clicked",
      //    ShortCode = link.ShortCode,
      //    LongUrl = link.LongUrl,
      //    ActivityTime = click.ClickedAt
      //  });
      //}
    }

    return activities
        .OrderByDescending(x => x.ActivityTime)
        .ToList();
  }
}
