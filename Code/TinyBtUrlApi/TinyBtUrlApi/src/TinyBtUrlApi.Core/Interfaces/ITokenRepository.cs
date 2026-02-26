using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface ITokenRepository
{
  Task<RefreshToken?> GetRefreshTokenAsync(string token);
  Task AddRevokedAccessTokenAsync(string token);
  Task AddRefreshTokenAsync(RefreshToken token);
  Task<bool> IsTokenRevoked(string token);
  Task SaveChangesAsync();
}
