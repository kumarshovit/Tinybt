using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Services;

public class TokenRepository : ITokenRepository
{
  private readonly AppDbContext _context;

  public TokenRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
  {
    return await _context.RefreshTokens
        .FirstOrDefaultAsync(r => r.Token == token);
  }

  public async Task AddRevokedAccessTokenAsync(string token)
  {
    await _context.RevokedTokens.AddAsync(new RevokedToken
    {
      Token = token,
      RevokedAt = DateTime.UtcNow
    });
  }

  public async Task AddRefreshTokenAsync(RefreshToken token)
  {
    await _context.RefreshTokens.AddAsync(token);
  }

  public async Task<bool> IsTokenRevoked(string token)
  {
    return await _context.RevokedTokens
        .AnyAsync(r => r.Token == token);
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
