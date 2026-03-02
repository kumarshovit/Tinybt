using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;


namespace TinyBtUrlApi.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
  private readonly AppDbContext _context;

  public SettingsRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<SystemSettings?> GetAsync()
  {
    return await _context.SystemSettings.FirstOrDefaultAsync();
  }

  public async Task UpdateAsync(SystemSettings settings)
  {
    if (settings.Id == 0)
      await _context.SystemSettings.AddAsync(settings);
    else
      _context.SystemSettings.Update(settings);

    await _context.SaveChangesAsync();
  }
}
