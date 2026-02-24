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
  private readonly IJwtService _jwtService;

  public LoginHandler(
      IRepository<User> userRepository,
      IRepository<RefreshToken> refreshTokenRepository,
      IJwtService jwtService)
  {
    _userRepository = userRepository;
    _refreshTokenRepository = refreshTokenRepository;
    _jwtService = jwtService;
  }

  public async Task<object> Handle(LoginQuery query)
  {
    var dto = query.Dto;

    var email = dto.Email!.Trim().ToLower();

    var user = await _userRepository.FirstOrDefaultAsync(
        new UserByEmailSpec(email)
    );

    if (user == null ||
        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
    {
      return new
      {
        message = "Invalid email or password."
      };
    }

    // 🔐 Generate Access Token
    var jwtResult = _jwtService.Generate(user);

    // 🔁 Generate Refresh Token
    var refreshTokenValue = Guid.NewGuid().ToString();

    var refreshToken = new RefreshToken
    {
      Token = refreshTokenValue,
      UserId = user.Id,
      ExpiresAt = DateTime.UtcNow.AddDays(7),
      IsRevoked = false
    };

    await _refreshTokenRepository.AddAsync(refreshToken);

    // 🎯 Return Tokens
    return new
    {
      accessToken = jwtResult.token,
      refreshToken = refreshTokenValue,
      expires = jwtResult.expires
    };
  }
}
