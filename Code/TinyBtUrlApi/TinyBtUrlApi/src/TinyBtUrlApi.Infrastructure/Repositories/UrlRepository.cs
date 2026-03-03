using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Infrastructure.Data;


namespace TinyBtUrlApi.Infrastructure.Data;

public class UrlRepository : IUrlRepository
{
  private readonly AppDbContext _context;

  public UrlRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(UrlMapping url)
  {
    _context.UrlMappings.Add(url);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(UrlMapping url)
  {
    _context.UrlMappings.Update(url);
    await _context.SaveChangesAsync();
  }

  public async Task<List<UrlMapping>> GetAllAsync()
  {
    return await _context.UrlMappings
        .Where(x => !x.IsDeleted)
        .Include(x => x.UrlTags)
        .ThenInclude(t => t.Tag)
        .OrderByDescending(x => x.CreatedAt)
        .ToListAsync();
  }

  public async Task<UrlMapping?> GetByIdAsync(int id)
  {
    return await _context.UrlMappings
        .Include(x => x.UrlTags)
        .ThenInclude(t => t.Tag)
        .FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<UrlMapping?> GetByShortCodeAsync(string shortCode)
  {
    return await _context.UrlMappings
        .FirstOrDefaultAsync(x => x.ShortCode == shortCode);
  }

  public async Task<bool> ShortCodeExists(string shortCode)
  {
    return await _context.UrlMappings
        .AnyAsync(x => x.ShortCode.ToLower() == shortCode.ToLower());
  }

  // ---------- TAGS ----------

  public async Task<List<UrlMapping>> SearchByTagAsync(string tag)
  {
    return await _context.UrlMappings
        .Include(u => u.UrlTags)
        .ThenInclude(ut => ut.Tag)
        .Where(u => u.UrlTags.Any(t => t.Tag.Name == tag.ToLower()))
        .ToListAsync();
  }

  public async Task AddTagsAsync(int urlId, List<string> tags)
  {
    var url = await _context.UrlMappings
        .Include(u => u.UrlTags)
        .FirstAsync(u => u.Id == urlId);

    foreach (var tagName in tags)
    {
      var normalized = tagName.Trim().ToLower();

      var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == normalized);
      if (tag == null)
      {
        tag = new Tag { Name = normalized };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
      }

      if (!url.UrlTags.Any(x => x.TagId == tag.Id))
      {
        url.UrlTags.Add(new UrlTag { UrlMappingId = urlId, TagId = tag.Id });
      }
    }

    await _context.SaveChangesAsync();
  }

  public async Task UpdateTagsAsync(int urlId, List<string> tags)
  {
    var url = await _context.UrlMappings
        .Include(u => u.UrlTags)
        .FirstAsync(u => u.Id == urlId);

    _context.UrlTags.RemoveRange(url.UrlTags);

    await AddTagsAsync(urlId, tags);
  }

  public async Task RemoveTagAsync(int urlId, string tagName)
  {
    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName.ToLower());
    if (tag == null) return;

    var urlTag = await _context.UrlTags
        .FirstOrDefaultAsync(x => x.UrlMappingId == urlId && x.TagId == tag.Id);

    if (urlTag != null)
    {
      _context.UrlTags.Remove(urlTag);
      await _context.SaveChangesAsync();
    }
  }

  public async Task RenameTagAsync(int urlId, string oldTag, string newTag)
  {
    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == oldTag.ToLower());
    if (tag == null) return;

    tag.Name = newTag.Trim().ToLower();
    await _context.SaveChangesAsync();
  }

  public async Task<int> GetTotalClicksAsync(CancellationToken cancellationToken)
  {
    return await _context.ClickLogs.CountAsync(cancellationToken);
  }

  public async Task<List<ClickOverTimeDto>> GetClicksOverTimeAsync(
    DateTime startDate,
    DateTime endDate,
    string viewType,
    CancellationToken cancellationToken)
  {
    var query = _context.ClickLogs
        .Where(x => x.ClickedAt >= startDate && x.ClickedAt <= endDate);

    // ✅ WEEKLY VIEW
    if (string.Equals(viewType, "weekly", StringComparison.OrdinalIgnoreCase))
    {
      return await query
          .GroupBy(x => EF.Functions.DateDiffWeek(startDate, x.ClickedAt))
          .OrderBy(g => g.Key) // 🔥 ORDER BEFORE SELECT (IMPORTANT)
          .Select(g => new ClickOverTimeDto
          {
            Period = "Week " + g.Key,
            Clicks = g.Count()
          })
          .ToListAsync(cancellationToken);
    }

    // ✅ DEFAULT: DAILY VIEW
    return await query
        .GroupBy(x => x.ClickedAt.Date)
        .OrderBy(g => g.Key) // 🔥 ORDER BEFORE SELECT (IMPORTANT)
        .Select(g => new ClickOverTimeDto
        {
          Period = g.Key.ToString("yyyy-MM-dd"),
          Clicks = g.Count()
        })
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ClicksByBrowserDto>> GetClicksByBrowserAsync(
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken)
  {
    return await _context.ClickLogs
        .Where(x => x.ClickedAt >= startDate && x.ClickedAt <= endDate)
        .GroupBy(x => x.Browser)
        .OrderByDescending(g => g.Count()) // sort by highest clicks
        .Select(g => new ClicksByBrowserDto
        {
          Browser = g.Key ?? "Unknown",
          Clicks = g.Count()
        })
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ClicksByCountryDto>> GetClicksByCountryAsync(
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken)
  {
    return await _context.ClickLogs
        .Where(x => x.ClickedAt >= startDate && x.ClickedAt <= endDate)
        .GroupBy(x => x.Country)
        .OrderByDescending(g => g.Count())
        .Select(g => new ClicksByCountryDto
        {
          Country = g.Key ?? "Unknown",
          Clicks = g.Count()
        })
        .ToListAsync(cancellationToken);
  }

  public async Task<List<TopUrlDto>> GetTopUrlsAsync(
    int topCount,
    CancellationToken cancellationToken)
  {
    return await _context.ClickLogs
        .GroupBy(c => c.ShortCode)
        .Select(g => new
        {
          ShortCode = g.Key,
          Clicks = g.Count()
        })
        .OrderByDescending(x => x.Clicks)
        .Take(topCount)
        .Join(_context.UrlMappings,
              click => click.ShortCode,
              url => url.ShortCode,
              (click, url) => new TopUrlDto
              {
                ShortCode = url.ShortCode,
                OriginalUrl = url.LongUrl,
                Clicks = click.Clicks
              })
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ClicksByDeviceLanguageDto>>
    GetClicksByDeviceLanguageAsync(CancellationToken cancellationToken)
  {
    return await _context.ClickLogs
        .GroupBy(x => string.IsNullOrEmpty(x.DeviceLanguage)
            ? "Unknown"
            : x.DeviceLanguage)
        .Select(g => new ClicksByDeviceLanguageDto
        {
          DeviceLanguage = g.Key!,
          Clicks = g.Count()
        })
        .OrderByDescending(x => x.Clicks)
        .ToListAsync(cancellationToken);
  }

  public async Task<int> GetActiveLinksAsync(CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;

    return await _context.UrlMappings
        .Where(x =>
            !x.IsDeleted &&
            (x.ExpirationDate == null || x.ExpirationDate > now))
        .CountAsync(cancellationToken);
  }

  public async Task<int> GetExpiredLinksAsync(CancellationToken cancellationToken)
  {
    var now = DateTime.UtcNow;

    return await _context.UrlMappings
        .Where(x =>
            !x.IsDeleted &&
            x.ExpirationDate != null &&
            x.ExpirationDate <= now)
        .CountAsync(cancellationToken);
  }

  public async Task<List<ClicksByOsDto>> GetClicksByOsAsync(
    CancellationToken cancellationToken)
  {
    return await _context.ClickLogs
        .GroupBy(x => string.IsNullOrEmpty(x.OS)
            ? "Unknown"
            : x.OS)
        .Select(g => new ClicksByOsDto
        {
          Os = g.Key!,
          Clicks = g.Count()
        })
        .OrderByDescending(x => x.Clicks)
        .ToListAsync(cancellationToken);
  }
  public async Task LogClickAsync(
    ClickLog clickLog,
    CancellationToken cancellationToken)
  {
    _context.ClickLogs.Add(clickLog);
    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task<List<ClicksByDeviceTypeDto>> GetClicksByDeviceTypeAsync(
    DateTime? startDate,
    DateTime? endDate,
    CancellationToken cancellationToken)
  {
    var query = _context.ClickLogs.AsQueryable();

    if (startDate.HasValue)
      query = query.Where(c => c.ClickedAt >= startDate.Value);

    if (endDate.HasValue)
      query = query.Where(c => c.ClickedAt <= endDate.Value);

    return await query
        .GroupBy(c => string.IsNullOrEmpty(c.DeviceType)
            ? "Unknown"
            : c.DeviceType)
        .Select(g => new ClicksByDeviceTypeDto
        {
          DeviceType = g.Key,
          Clicks = g.Count()
        })
        .OrderByDescending(x => x.Clicks)
        .ToListAsync(cancellationToken);
  }

  public async Task<List<ClickLog>> GetClickLogsByShortCodeAsync(
    string shortCode,
    CancellationToken ct)
  {
    return await _context.ClickLogs
        .Where(x => x.ShortCode == shortCode)
        .ToListAsync(ct);
  }

  public async Task<List<ClickOverTimeDto>> GetLinkClicksOverTimeAsync(
    string shortCode,
    DateTime startDate,
    DateTime endDate,
    string viewType,
    CancellationToken cancellationToken)
  {
    var query = _context.ClickLogs
        .Where(x =>
            x.ShortCode == shortCode &&
            x.ClickedAt >= startDate &&
            x.ClickedAt < endDate.AddDays(1));

    if (viewType == "Monthly")
    {
      var data = await query
          .GroupBy(x => new { x.ClickedAt.Year, x.ClickedAt.Month })
          .Select(g => new
          {
            g.Key.Year,
            g.Key.Month,
            Clicks = g.Count()
          })
          .OrderBy(x => x.Year)
          .ThenBy(x => x.Month)
          .ToListAsync(cancellationToken);

      return data.Select(x => new ClickOverTimeDto
      {
        Period = $"{x.Year}-{x.Month:D2}",
        Clicks = x.Clicks
      }).ToList();
    }

    if (viewType == "Weekly")
    {
      var data = await query
          .GroupBy(x => EF.Functions.DateDiffWeek(startDate, x.ClickedAt))
          .Select(g => new
          {
            Week = g.Key,
            Clicks = g.Count()
          })
          .OrderBy(x => x.Week)
          .ToListAsync(cancellationToken);

      return data.Select(x => new ClickOverTimeDto
      {
        Period = $"Week {x.Week}",
        Clicks = x.Clicks
      }).ToList();
    }

    // ✅ Default Daily
    var dailyData = await query
        .GroupBy(x => new
        {
          x.ClickedAt.Year,
          x.ClickedAt.Month,
          x.ClickedAt.Day
        })
        .Select(g => new
        {
          g.Key.Year,
          g.Key.Month,
          g.Key.Day,
          Clicks = g.Count()
        })
        .OrderBy(x => x.Year)
        .ThenBy(x => x.Month)
        .ThenBy(x => x.Day)
        .ToListAsync(cancellationToken);

    return dailyData.Select(x => new ClickOverTimeDto
    {
      Period = $"{x.Year}-{x.Month:D2}-{x.Day:D2}",
      Clicks = x.Clicks
    }).ToList();
  }
}
