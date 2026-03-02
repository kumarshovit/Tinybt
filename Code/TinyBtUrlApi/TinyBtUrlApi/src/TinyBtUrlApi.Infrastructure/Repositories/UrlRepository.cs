using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


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
}
