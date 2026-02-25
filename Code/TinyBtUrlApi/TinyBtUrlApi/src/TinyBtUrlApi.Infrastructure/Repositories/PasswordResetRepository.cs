using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

public class PasswordResetRepository : IPasswordResetRepository
{
  private readonly AppDbContext _context;

  public PasswordResetRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task AddAsync(PasswordResetToken token)
  {
    await _context.PasswordResetTokens.AddAsync(token);
  }

  public async Task<PasswordResetToken?> GetByTokenAsync(string token)
  {
    return await _context.PasswordResetTokens
        .FirstOrDefaultAsync(t => t.Token == token);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
