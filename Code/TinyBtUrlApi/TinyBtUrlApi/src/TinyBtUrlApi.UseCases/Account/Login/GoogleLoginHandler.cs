using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class GoogleLoginHandler
{
  private readonly IRepository<User> _repository;
  private readonly IGoogleAuthService _googleService;
  private readonly IJwtService _jwtService;

  public GoogleLoginHandler(
      IRepository<User> repository,
      IGoogleAuthService googleService,
      IJwtService jwtService)
  {
    _repository = repository;
    _googleService = googleService;
    _jwtService = jwtService;
  }

  public async Task<(string token, DateTime expires)?> Handle(GoogleLoginQuery request)
  {
    var googleUser = await _googleService
        .ValidateTokenAsync(request.IdToken);

    if (googleUser == null)
      return null;

    var spec = new UserByEmailSpec(googleUser.Email!);
    var existingUser = await _repository.FirstOrDefaultAsync(spec);

    if (existingUser != null)
    {
      if (existingUser.LockoutEnd != null &&
          existingUser.LockoutEnd > DateTime.UtcNow)
      {
        return null;
      }

      // ✅ Use existing JWT method
      return _jwtService.Generate(existingUser);
    }

    // 🆕 Create new user
    var newUser = new User
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

    await _repository.AddAsync(newUser);

    return _jwtService.Generate(newUser);
  }
}
