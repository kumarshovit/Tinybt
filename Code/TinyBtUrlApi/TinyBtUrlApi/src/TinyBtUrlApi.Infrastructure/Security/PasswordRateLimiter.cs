using System;
using Microsoft.Extensions.Caching.Memory;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Security;

public class PasswordRateLimiter : IPasswordRateLimiter
{
    private readonly IMemoryCache _cache;
    private const int MaxAttempts = 5;
    private static readonly TimeSpan RateLimitWindow = TimeSpan.FromMinutes(15);

    public PasswordRateLimiter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool IsAllowed(string shortCode, string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return true;

        var cacheKey = $"pwd_attempts:{shortCode}:{ipAddress}";
        var attempts = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = RateLimitWindow;
            return 0;
        });

        return attempts < MaxAttempts;
    }

    public void RecordFailedAttempt(string shortCode, string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return;

        var cacheKey = $"pwd_attempts:{shortCode}:{ipAddress}";
        
        _cache.Set(cacheKey,
            _cache.GetOrCreate(cacheKey, e =>
            {
                e.AbsoluteExpirationRelativeToNow = RateLimitWindow;
                return 0;
            }) + 1,
            RateLimitWindow);
    }
}
