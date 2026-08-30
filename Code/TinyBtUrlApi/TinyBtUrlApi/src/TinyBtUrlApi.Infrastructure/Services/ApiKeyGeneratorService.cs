using System;
using System.Security.Cryptography;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ApiKeyGeneratorService : IApiKeyGeneratorService
{
    private const string Prefix = "lbt_live_";
    private const int SecretByteLength = 32;

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
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToBase64String(hashedBytes);
    }
}
