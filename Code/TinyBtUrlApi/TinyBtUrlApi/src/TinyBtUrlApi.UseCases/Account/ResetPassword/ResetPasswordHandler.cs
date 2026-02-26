using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.ResetPassword;

public class ResetPasswordHandler
{
  private readonly IRepository<User> _userRepo;
  private readonly IPasswordResetRepository _resetRepo;

  public ResetPasswordHandler(
      IRepository<User> userRepo,
      IPasswordResetRepository resetRepo)
  {
    _userRepo = userRepo;
    _resetRepo = resetRepo;
  }

  public async Task<bool> Handle(ResetPasswordQuery request)
  {
    var resetToken =
        await _resetRepo.GetByTokenAsync(request.Token);

    if (resetToken == null ||
        resetToken.IsUsed ||
        resetToken.ExpiresAt < DateTime.UtcNow)
      return false;

    var user = await _userRepo
        .FirstOrDefaultAsync(new UserByEmailSpec(request.Email));

    if (user == null)
      return false;

    // 🔐 Password Strength Validation
    if (request.NewPassword.Length < 8)
      throw new Exception("Password must be at least 8 characters.");

    user.PasswordHash =
        BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

    resetToken.IsUsed = true;

    await _resetRepo.SaveChangesAsync();

    return true;
  }
}
