using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using System;
using System.Linq;

namespace TinyBtUrlApi.Web.Auth.ApiKey;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IApiKeyGeneratorService _apiKeyGeneratorService;
    private readonly TimeProvider _timeProvider;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider timeProvider,
        IApiKeyRepository apiKeyRepository,
        IApiKeyGeneratorService apiKeyGeneratorService)
        : base(options, logger, encoder)
    {
        _apiKeyRepository = apiKeyRepository;
        _apiKeyGeneratorService = apiKeyGeneratorService;
        _timeProvider = timeProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(providedApiKey) || !providedApiKey.StartsWith("lbt_live_"))
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // 1. Hash incoming key
        var incomingHash = _apiKeyGeneratorService.HashKey(providedApiKey);

        // 2. Lookup by Hash
        var apiKeyEntity = await _apiKeyRepository.FindByHashAsync(incomingHash, Context.RequestAborted);

        if (apiKeyEntity == null)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // 3. Check Revoked
        if (apiKeyEntity.RevokedAt != null)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // 4. Check Expired
        if (apiKeyEntity.ExpiresAt != null && apiKeyEntity.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // 5. Success -> Create Identity
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKeyEntity.UserId.ToString()),
            new Claim(ClaimTypes.Name, apiKeyEntity.Name),
            new Claim("ApiKeyId", apiKeyEntity.Id.ToString())
        };

    //var identity = new ClaimsIdentity(claims, Options.DefaultScheme);
    //var principal = new ClaimsPrincipal(identity);
    //var ticket = new AuthenticationTicket(principal, Options.DefaultScheme);

    var identity = new ClaimsIdentity(
    claims,
    ApiKeyAuthenticationOptions.DefaultScheme);

    var principal = new ClaimsPrincipal(identity);

    var ticket = new AuthenticationTicket(
        principal,
        ApiKeyAuthenticationOptions.DefaultScheme);

    return AuthenticateResult.Success(ticket);
    }
}
