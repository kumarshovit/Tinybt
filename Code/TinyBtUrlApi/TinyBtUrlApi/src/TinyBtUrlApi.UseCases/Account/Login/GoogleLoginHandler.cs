using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class GoogleLoginHandler
{
  private readonly IRepository<User> _repository;
  private readonly IGoogleAuthService _googleService;
  private readonly IJwtService _jwtService;
  private readonly ITokenRepository _tokenRepository;

  public GoogleLoginHandler(
      IRepository<User> repository,
      IGoogleAuthService googleService,
      IJwtService jwtService,
      ITokenRepository tokenRepository)
  {
    _repository = repository;
    _googleService = googleService;
    _jwtService = jwtService;
    _tokenRepository = tokenRepository;
  }

  public async Task<(string token, string refreshToken, DateTime expires)?>
      Handle(GoogleLoginQuery request)
  {
    // 1️⃣ Validate Google Token
    var googleUser = await _googleService
        .ValidateTokenAsync(request.IdToken);

    if (googleUser == null)
      return null;

    // 2️⃣ Check if user already exists
    var spec = new UserByEmailSpec(googleUser.Email!);
    var user = await _repository.FirstOrDefaultAsync(spec);
    // 🚨 BLOCK DELETED USERS
    if (user != null && user.IsDeleted)
    {
      return null;
    }
    // 3️⃣ If user not found → create new
    if (user == null)
    {
      user = new User
      {
        Email = googleUser.Email,
        FullName = googleUser.FullName,
        GoogleId = googleUser.GoogleId,
        IsGoogleAccount = true,
        LoginProvider = "Google",
        IsEmailVerified = true,
        PasswordHash = BCrypt.Net.BCrypt
              .HashPassword(Guid.NewGuid().ToString())
      };

      await _repository.AddAsync(user);
    }
    else
    {
      // Optional: Lockout check
      if (user.LockoutEnd != null &&
          user.LockoutEnd > DateTime.UtcNow)
      {
        return null;
      }
    }

    // 4️⃣ Generate Access Token
    var (accessToken, expires) = _jwtService.Generate(user);

    // 5️⃣ Generate Refresh Token
    var refreshTokenValue = Guid.NewGuid().ToString();

    var refreshToken = new RefreshToken
    {
      Token = refreshTokenValue,
      UserId = user.Id,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    await _tokenRepository.AddRefreshTokenAsync(refreshToken);
    await _tokenRepository.SaveChangesAsync();

    // 6️⃣ Return both tokens
    return (accessToken, refreshTokenValue, expires);
  }
}
