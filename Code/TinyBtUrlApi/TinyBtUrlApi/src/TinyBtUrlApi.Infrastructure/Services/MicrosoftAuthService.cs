using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Services;

public class MicrosoftAuthService : IMicrosoftAuthService
{
  private readonly IConfiguration _configuration;
  private readonly ILogger<MicrosoftAuthService> _logger;
  private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
  private readonly JwtSecurityTokenHandler _tokenHandler;

  public MicrosoftAuthService(IConfiguration configuration, ILogger<MicrosoftAuthService> logger)
  {
    _configuration = configuration;
    _logger = logger;

    // Microsoft v2.0 multi-tenant / personal accounts discovery endpoint
    var metadataAddress = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
    _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        metadataAddress,
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever());

    _tokenHandler = new JwtSecurityTokenHandler();
  }

  public async Task<User?> ValidateTokenAsync(string idToken)
  {
    if (string.IsNullOrWhiteSpace(idToken))
      return null;

    try
    {
      var clientId = _configuration["MicrosoftAuth:ClientId"];
      if (string.IsNullOrWhiteSpace(clientId))
      {
        _logger.LogError("MicrosoftAuth:ClientId is missing in application configuration.");
        return null;
      }

      // Fetch signing keys from Microsoft's OpenID configuration (cached automatically by ConfigurationManager)
      var discoveryDocument = await _configurationManager.GetConfigurationAsync(CancellationToken.None);

      var validationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = discoveryDocument.SigningKeys,

        ValidateAudience = true,
        ValidAudience = clientId,

        ValidateIssuer = true,
        IssuerValidator = (issuer, securityToken, parameters) =>
        {
          if (!string.IsNullOrEmpty(issuer) &&
              issuer.StartsWith("https://login.microsoftonline.com/", StringComparison.OrdinalIgnoreCase) &&
              issuer.EndsWith("/v2.0", StringComparison.OrdinalIgnoreCase))
          {
            return issuer;
          }
          throw new SecurityTokenInvalidIssuerException($"Invalid Microsoft issuer: {issuer}");
        },

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
      };

      var principal = _tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);

      // Unique Microsoft identifier (oid is preferred, fallback to sub / nameidentifier)
      var microsoftId = principal.FindFirstValue("oid")
          ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
          ?? principal.FindFirstValue("sub");

      if (string.IsNullOrWhiteSpace(microsoftId))
      {
        _logger.LogWarning("Microsoft token validated successfully but missing user identifier claim (oid/sub).");
        return null;
      }

      // Extract email from email or preferred_username claim
      var email = principal.FindFirstValue(ClaimTypes.Email)
          ?? principal.FindFirstValue("preferred_username")
          ?? principal.FindFirstValue("email");

      if (string.IsNullOrWhiteSpace(email))
      {
        _logger.LogWarning("Microsoft token validated successfully but missing email claim.");
        return null;
      }

      var name = principal.FindFirstValue(ClaimTypes.Name)
          ?? principal.FindFirstValue("name")
          ?? email.Split('@')[0];

      return new User
      {
        Email = email.ToLowerInvariant(),
        FullName = name,
        MicrosoftId = microsoftId,
        IsMicrosoftAccount = true,
        IsEmailVerified = true,
        LoginProvider = "Microsoft",
        PasswordHash = string.Empty,
        CreatedAt = DateTime.UtcNow,
        Role = "User",
        IsActive = true
      };
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Failed to validate Microsoft ID token.");
      return null;
    }
  }
}

