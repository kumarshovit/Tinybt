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
    var clientId = _configuration["GoogleAuth:ClientId"];

    var payload = await GoogleJsonWebSignature.ValidateAsync(
        idToken,
        new GoogleJsonWebSignature.ValidationSettings
        {
          Audience = new[] { clientId }
        });

    if (!payload.EmailVerified)
      return null;

    // ✅ Object Initializer (NO constructor required)
    return new User
    {
      Email = payload.Email.ToLower(),
      FullName = payload.Name,
      GoogleId = payload.Subject,      // Unique Google user ID
      IsGoogleAccount = true,
      IsEmailVerified = true,
      LoginProvider = "Google",
      PasswordHash = string.Empty,     // Because Google login
      CreatedAt = DateTime.UtcNow,
      Role = "User"
    };
  }
}
