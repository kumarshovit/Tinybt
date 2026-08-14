using System;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Security;

/// <summary>
/// Concrete implementation of token access using ASP.NET Core Data Protection.
/// </summary>
public class UrlAccessTokenService : IUrlAccessTokenService
{
    private const string Purpose = "LinkBT-UrlAccess-v1";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(5);
    // Reject tokens issued more than 30 s in the future (clock skew guard)
    private static readonly TimeSpan MaxFutureDrift = TimeSpan.FromSeconds(30);

    private readonly IDataProtectionProvider _provider;

    public UrlAccessTokenService(IDataProtectionProvider provider)
    {
        _provider = provider;
    }

    public string Protect(string shortCode, string source)
    {
        var payload = new AccessTokenPayload(
            shortCode,
            source,
            DateTime.UtcNow.Ticks);

        var json = JsonSerializer.Serialize(payload);
        return _provider.CreateProtector(Purpose).Protect(json);
    }

    public AccessTokenPayload? TryUnprotect(string token, string expectedShortCode)
    {
        try
        {
            var json = _provider.CreateProtector(Purpose).Unprotect(token);
            var payload = JsonSerializer.Deserialize<AccessTokenPayload>(json);

            if (payload is null)
                return null;

            // Validate shortCode match
            if (!string.Equals(payload.ShortCode, expectedShortCode,
                    StringComparison.Ordinal))
                return null;

            var issuedAt = new DateTime(payload.IssuedAtTicks, DateTimeKind.Utc);
            var now = DateTime.UtcNow;

            // Reject expired tokens
            if (now - issuedAt > TokenLifetime)
                return null;

            // Reject tokens issued too far in the future
            if (issuedAt - now > MaxFutureDrift)
                return null;

            return payload;
        }
        catch
        {
            // CryptographicException = tampered / unknown key
            return null;
        }
    }
}
