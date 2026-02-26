using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.ForgotPassword;

public class ForgotPasswordHandler
{
  private readonly IRepository<User> _userRepo;
  private readonly IPasswordResetRepository _resetRepo;
  private readonly IEmailSender _emailSender;

  public ForgotPasswordHandler(
      IRepository<User> userRepo,
      IPasswordResetRepository resetRepo,
       IEmailSender emailSender)
  {
    _userRepo = userRepo;
    _resetRepo = resetRepo;
    _emailSender = emailSender;
  }

  public async Task Handle(ForgotPasswordQuery request)
  {
    var user = await _userRepo
        .FirstOrDefaultAsync(new UserByEmailSpec(request.Email));

    if (user == null)
      return; // don't reveal user existence

    var tokenValue = Guid.NewGuid().ToString();

    var resetToken = new PasswordResetToken
    {
      Token = tokenValue,
      UserId = user.Id,
      ExpiresAt = DateTime.UtcNow.AddMinutes(15)
    };

    await _resetRepo.AddAsync(resetToken);
    await _resetRepo.SaveChangesAsync();

    var resetLink =
        $"http://localhost:5173/reset-password?email={request.Email}&token={tokenValue}";

    await _emailSender.SendEmailAsync(
      request.Email,                 // to
      "kg834208@gmail.com",         // from
      "Reset Your Password",         // subject
      $"Click the link below:\n{resetLink}"  // body
  );
  }
}
