using System.Threading;
using System.Threading.Tasks;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey> CreateAsync(int userId, string name, ApiKeyGenerationResult keyInfo, CancellationToken ct = default);
    Task<ApiKey?> FindByHashAsync(string keyHash, CancellationToken ct = default);
    Task<System.Collections.Generic.List<ApiKey>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<bool> RevokeAsync(int id, int userId, CancellationToken ct = default);
    Task UpdateLastUsedAtAsync(int id, DateTime lastUsedAt, CancellationToken ct = default);
    Task<int> CountActiveByUserIdAsync(int userId, CancellationToken ct = default);
}
