using Google.Apis.Auth;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

public class GoogleAuthService : IGoogleAuthService
{
  private readonly IConfiguration _configuration;

  public GoogleAuthService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public async Task<User?> ValidateTokenAsync(string idToken)
  {
    if (string.IsNullOrWhiteSpace(idToken))
      return null;

    try
    {
      var clientId = _configuration["GoogleAuth:ClientId"];

      var validationSettings = new GoogleJsonWebSignature.ValidationSettings();
      if (!string.IsNullOrWhiteSpace(clientId))
      {
        validationSettings.Audience = new[] { clientId };
      }

      var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

      if (payload == null || !payload.EmailVerified || string.IsNullOrWhiteSpace(payload.Email))
        return null;

      // ✅ Object Initializer
      return new User
      {
        Email = payload.Email.ToLowerInvariant(),
        FullName = payload.Name ?? payload.Email,
        GoogleId = payload.Subject,      // Unique Google user ID
        IsGoogleAccount = true,
        IsEmailVerified = true,
        LoginProvider = "Google",
        PasswordHash = string.Empty,     // OAuth account
        CreatedAt = DateTime.UtcNow,
        Role = "User",
        IsActive = true
      };
    }
    catch (Exception)
    {
      return null;
    }
  }
}
