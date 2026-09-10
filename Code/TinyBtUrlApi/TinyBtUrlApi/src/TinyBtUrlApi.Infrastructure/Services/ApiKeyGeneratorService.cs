using System;
using System.Security.Cryptography;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ApiKeyGeneratorService : IApiKeyGeneratorService
{
    private const string Prefix = "lbt_live_";
    private const int SecretByteLength = 32;
    private readonly string _hashSecret;

    public ApiKeyGeneratorService(IConfiguration configuration)
    {
        _hashSecret = configuration["ApiKey:HashSecret"] ?? "default_fallback_secret_for_dev_mode_only_123!";
    }

    public ApiKeyGenerationResult GenerateKey()
    {
        // 1. Generate cryptographically secure random bytes
        var randomBytes = new byte[SecretByteLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        // 2. Base64Url encode it so it's URL-safe and easy to copy
        var randomString = WebEncoders.Base64UrlEncode(randomBytes);
        
        // 3. Construct raw key
        var rawKey = $"{Prefix}{randomString}";

        // 4. Calculate hash
        var hash = HashKey(rawKey);

        // 5. Create display prefix (Prefix + first 8 characters of random string)
        var displayPrefix = $"{Prefix}{randomString.Substring(0, 8)}";

        return new ApiKeyGenerationResult
        {
            RawKey = rawKey,
            Prefix = displayPrefix,
            Hash = hash
        };
    }

    public string HashKey(string rawKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hashSecret));
        var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToBase64String(hashedBytes);
    }

    public string LegacyHashKey(string rawKey)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToBase64String(hashedBytes);
    }
}
