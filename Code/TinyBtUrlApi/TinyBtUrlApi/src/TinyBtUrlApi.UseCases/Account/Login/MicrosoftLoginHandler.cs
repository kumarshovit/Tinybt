using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class MicrosoftLoginHandler
{
  private readonly IRepository<User> _repository;
  private readonly IMicrosoftAuthService _microsoftService;
  private readonly IJwtService _jwtService;
  private readonly ITokenRepository _tokenRepository;

  public MicrosoftLoginHandler(
      IRepository<User> repository,
      IMicrosoftAuthService microsoftService,
      IJwtService jwtService,
      ITokenRepository tokenRepository)
  {
    _repository = repository;
    _microsoftService = microsoftService;
    _jwtService = jwtService;
    _tokenRepository = tokenRepository;
  }

  public async Task<(string token, string refreshToken, DateTime expires)?>
      Handle(MicrosoftLoginQuery request)
  {
    // 1️⃣ Validate Microsoft ID Token
    var microsoftUser = await _microsoftService
        .ValidateTokenAsync(request.IdToken);

    if (microsoftUser == null)
      return null;

    // 2️⃣ Check if user already exists by MicrosoftId
    var specById = new UserByMicrosoftIdSpec(microsoftUser.MicrosoftId!);
    var user = await _repository.FirstOrDefaultAsync(specById);

    if (user == null)
    {
      // Check if user exists by verified Email (Account Linking)
      var specByEmail = new UserByEmailSpec(microsoftUser.Email!);
      user = await _repository.FirstOrDefaultAsync(specByEmail);

      if (user != null)
      {
        // Link Microsoft account to existing user
        user.MicrosoftId = microsoftUser.MicrosoftId;
        user.IsMicrosoftAccount = true;
        if (!user.IsEmailVerified)
        {
          user.IsEmailVerified = true;
        }
        await _repository.UpdateAsync(user);
      }
    }

    // 🚨 BLOCK DELETED OR INACTIVE USERS
    if (user != null && (user.IsDeleted || !user.IsActive))
    {
      return null;
    }

    // 3️⃣ If user not found at all → create new user
    if (user == null)
    {
      user = new User
      {
        Email = microsoftUser.Email,
        FullName = microsoftUser.FullName,
        MicrosoftId = microsoftUser.MicrosoftId,
        IsMicrosoftAccount = true,
        LoginProvider = "Microsoft",
        IsEmailVerified = true,
        PasswordHash = BCrypt.Net.BCrypt
              .HashPassword(Guid.NewGuid().ToString()),
        CreatedAt = DateTime.UtcNow,
        Role = "User",
        IsActive = true
      };

      await _repository.AddAsync(user);
    }
    else
    {
      // Optional: Lockout check
      if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
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

