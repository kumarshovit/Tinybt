using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Repositories;

public class ApiKeyRepository : IApiKeyRepository
{
    private readonly AppDbContext _dbContext;

    public ApiKeyRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiKey> CreateAsync(int userId, string name, ApiKeyGenerationResult keyInfo, CancellationToken ct = default)
    {
        var apiKey = new ApiKey
        {
            UserId = userId,
            Name = name,
            KeyPrefix = keyInfo.Prefix,
            KeyHash = keyInfo.Hash,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ApiKeys.Add(apiKey);
        await _dbContext.SaveChangesAsync(ct);

        return apiKey;
    }

    public async Task<ApiKey?> FindByHashAsync(string keyHash, CancellationToken ct = default)
    {
        return await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.KeyHash == keyHash, ct);
    }

    public async Task<System.Collections.Generic.List<ApiKey>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _dbContext.ApiKeys
            .AsNoTracking()
            .Where(k => k.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<bool> RevokeAsync(int id, int userId, CancellationToken ct = default)
    {
        var apiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId, ct);

        if (apiKey == null)
        {
            return false;
        }

        if (apiKey.RevokedAt != null)
        {
            return true; // Already revoked
        }

        apiKey.RevokedAt = DateTime.UtcNow;
        _dbContext.Update(apiKey);
        await _dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task UpdateLastUsedAtAsync(int id, DateTime lastUsedAt, CancellationToken ct = default)
    {
        await _dbContext.ApiKeys
            .Where(k => k.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(k => k.LastUsedAt, lastUsedAt), ct);
    }

    public async Task<int> CountActiveByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _dbContext.ApiKeys
            .CountAsync(k => k.UserId == userId && k.RevokedAt == null, ct);
    }
}
