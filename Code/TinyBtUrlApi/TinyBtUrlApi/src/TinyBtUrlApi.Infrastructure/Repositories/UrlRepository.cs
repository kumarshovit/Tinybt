using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

internal class UrlRepository : IUrlRepository
{
  private readonly AppDbContext _context;

  public UrlRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(UrlMapping url)
  {
    await _context.UrlMappings.AddAsync(url);
  }

  public async Task<UrlMapping?> GetByShortCodeAsync(string shortCode)
  {
    return await _context.UrlMappings
        .FirstOrDefaultAsync(x => x.ShortCode == shortCode);
  }

  public async Task<List<UrlMapping>> GetAllAsync()
  {
    return await _context.UrlMappings.ToListAsync();
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
