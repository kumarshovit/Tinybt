//using Microsoft.EntityFrameworkCore;
//using TinyBtUrlApi.Core.Interfaces;
//using TinyBtUrlApi.Infrastructure.Data;

//namespace TinyBtUrlApi.Infrastructure.Repositories;

//public sealed class AnalyticsRepository(AppDbContext context)
//    : IAnalyticsRepository
//{
//  public async Task<int> GetTotalClicksAsync(
//      DateTime from, DateTime to, CancellationToken ct)
//  {
//    return await context.ClickLogs
//        .AsNoTracking()
//        .Where(x => x.ClickedAt >= from && x.ClickedAt <= to)
//        .CountAsync(ct);
//  }

//  public async Task<int> GetUniqueVisitorsAsync(
//      DateTime from, DateTime to, CancellationToken ct)
//  {
//    return await context.ClickLogs
//        .AsNoTracking()
//        .Where(x => x.ClickedAt >= from && x.ClickedAt <= to)
//        .Select(x => x.VisitorId)
//        .Distinct()
//        .CountAsync(ct);
//  }

//  public async Task<int> GetTotalUrlsAsync(CancellationToken ct)
//  {
//    return await context.UrlMappings
//        .AsNoTracking()
//        .CountAsync(ct);
//  }

//  public async Task<int> GetTotalTagsAsync(CancellationToken ct)
//  {
//    return await context.Tags
//        .AsNoTracking()
//        .CountAsync(ct);
//  }

//  public async Task<int> GetActiveLinksAsync(CancellationToken ct)
//  {
//    var now = DateTime.UtcNow;

//    return await context.UrlMappings
//        .AsNoTracking()
//        .Where(x =>
//            !x.IsDeleted &&
//            (x.ExpirationDate == null || x.ExpirationDate > now))
//        .CountAsync(ct);
//  }

//  public async Task<int> GetExpiredLinksAsync(CancellationToken ct)
//  {
//    var now = DateTime.UtcNow;

//    return await context.UrlMappings
//        .AsNoTracking()
//        .Where(x =>
//            !x.IsDeleted &&
//            x.ExpirationDate != null &&
//            x.ExpirationDate <= now)
//        .CountAsync(ct);
//  }
//}


using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.DTOs;
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

  public async Task<List<UserActivityDto>> GetUserActivityAsync(int userId, CancellationToken ct)
  {
    var activities = new List<UserActivityDto>();

    var links = await context.UrlMappings
        .AsNoTracking()
        .Where(x => x.UserId == userId)
        .ToListAsync(ct);

    foreach (var link in links)
    {
      activities.Add(new UserActivityDto
      {
        ActivityType = "Link Created",
        ShortCode = link.ShortCode,
        LongUrl = link.LongUrl,
        ActivityTime = link.CreatedAt
      });

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

      //if (link.ClickedAt != null)
      //{
      //  activities.Add(new UserActivityDto
      //  {
      //    ActivityType = "Link Clicked",
      //    ShortCode = link.ShortCode,
      //    LongUrl = link.LongUrl,
      //    ActivityTime = link.ClickedAt.Value
      //  });
      //}
    }

    return activities
        .OrderByDescending(x => x.ActivityTime)
        .ToList();
  }
}
