using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class LoginHandler
{
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<RefreshToken> _refreshTokenRepository;
  private readonly IRepository<IpLoginAttempt> _ipRepository;
  private readonly IJwtService _jwtService;

  public LoginHandler(
      IRepository<User> userRepository,
      IRepository<RefreshToken> refreshTokenRepository,
      IRepository<IpLoginAttempt> ipRepository,
      IJwtService jwtService)
  {
    _userRepository = userRepository;
    _refreshTokenRepository = refreshTokenRepository;
    _ipRepository = ipRepository;
    _jwtService = jwtService;
  }

  public async Task<object> Handle(LoginQuery query)
  {
    var dto = query.Dto;
    var ip = query.IpAddress;

    var ipAttempt = await _ipRepository
        .FirstOrDefaultAsync(new IpLoginAttemptByIpSpec(ip));

    // 🔒 Already blocked
    if (ipAttempt != null &&
        ipAttempt.BlockedUntil != null &&
        ipAttempt.BlockedUntil > DateTime.UtcNow)
    {
      return new
      {
        message = $"Too many attempts. Try again after {ipAttempt.BlockedUntil}",
        requireCaptcha = true
      };
    }

    var email = dto.Email!.Trim().ToLower();

    var user = await _userRepository
        .FirstOrDefaultAsync(new UserByEmailSpec(email));

    // ❌ Login Failure
    if (user == null ||
        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
    {

      if (ipAttempt == null)
      {
        ipAttempt = new IpLoginAttempt
        {
          IpAddress = ip,
          AttemptCount = 1,
          LastAttemptAt = DateTime.UtcNow
        };

        await _ipRepository.AddAsync(ipAttempt);
      }
      else
      {
        ipAttempt.AttemptCount++;
        ipAttempt.LastAttemptAt = DateTime.UtcNow;

        if (ipAttempt.AttemptCount >= 5)
        {
          ipAttempt.BlockedUntil = DateTime.UtcNow.AddMinutes(10);

          await _ipRepository.UpdateAsync(ipAttempt);

          return new
          {
            message = "Too many attempts. You are blocked for 10 minutes.",
            requireCaptcha = true
          };
        }

        await _ipRepository.UpdateAsync(ipAttempt);
      }

      return new
      {
        message = "Invalid email or password."
      };
    }

    if (!user.IsActive)
    {
      return new
      {
        message = "Your account has been disabled by admin"
      };
    }
    // 🔄 Reset IP attempts on success
    if (ipAttempt != null)
    {
      ipAttempt.AttemptCount = 0;
      ipAttempt.BlockedUntil = null;
      await _ipRepository.UpdateAsync(ipAttempt);
    }

    if (!user.IsEmailVerified)
    {
      return new
      {
        message = "Please verify your email before logging in."
      };
    }

    // 🔐 Generate tokens
    var (token, expires) = _jwtService.Generate(user);

    var refreshTokenValue = Guid.NewGuid().ToString();

    var refreshToken = new RefreshToken
    {
      Token = refreshTokenValue,
      UserId = user.Id,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(refreshToken);

    return new
    {
      accessToken = token,
      refreshToken = refreshTokenValue,
      expires = expires
    };
  }
}
