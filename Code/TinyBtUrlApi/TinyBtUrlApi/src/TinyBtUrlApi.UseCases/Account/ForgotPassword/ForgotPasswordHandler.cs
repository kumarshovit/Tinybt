//using System;
//using System.Collections.Generic;
//using System.Text;
//using TinyBtUrlApi.Core.Interfaces;
//using TinyBtUrlApi.Core.Models;
//using TinyBtUrlApi.Core.Specifications;

//namespace TinyBtUrlApi.UseCases.Account.ForgotPassword;

//public class ForgotPasswordHandler
//{
//  private readonly IRepository<User> _userRepo;
//  private readonly IPasswordResetRepository _resetRepo;
//  private readonly IEmailSender _emailSender;
//  private readonly string _frontendUrl;

//  public ForgotPasswordHandler(
//      IRepository<User> userRepo,
//      IPasswordResetRepository resetRepo,
//       IEmailSender emailSender,
//        string frontendUrl)
//  {
//    _userRepo = userRepo;
//    _resetRepo = resetRepo;
//    _emailSender = emailSender;
//    _frontendUrl = frontendUrl;
//  }

//  public async Task Handle(ForgotPasswordQuery request)
//  {
//    var user = await _userRepo
//        .FirstOrDefaultAsync(new UserByEmailSpec(request.Email));

//    if (user == null)
//      return; // don't reveal user existence

//    var tokenValue = Guid.NewGuid().ToString();

//    var resetToken = new PasswordResetToken
//    {
//      Token = tokenValue,
//      UserId = user.Id,
//      ExpiresAt = DateTime.UtcNow.AddMinutes(15)
//    };

//    await _resetRepo.AddAsync(resetToken);
//    await _resetRepo.SaveChangesAsync();

//    var resetLink =
//        $"{_frontendUrl}/reset-password?email={request.Email}&token={tokenValue}";
//    var emailBody = $@"
//<html>
//  <body style='font-family: Arial; background-color:#f4f6f8; padding:20px;'>
//    <div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:10px;'>

//      <h2>Reset Your Password</h2>

//      <p>Hello,</p>

//      <p>You requested to reset your password. Click the button below to proceed.</p>

//      <div style='text-align:center; margin:30px 0;'>
//        <a href='{resetLink}' 
//           style='background-color:#ff5722; color:white; padding:12px 20px; text-decoration:none; border-radius:5px;'>
//           Reset Password
//        </a>
//      </div>

//      <p>This link will expire in 15 minutes.</p>

//      <p>If you didn’t request this, you can ignore this email.</p>

//    </div>
//  </body>
//</html>";

//    await _emailSender.SendEmailAsync(
//      request.Email,
//      "kg834208@gmail.com",
//      "Reset Your Password",
//      emailBody
//    );
//  }
//}


using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.ForgotPassword;

public class ForgotPasswordHandler
{
  private readonly IRepository<User> _userRepo;
  private readonly IPasswordResetRepository _resetRepo;
  private readonly IEmailSender _emailSender;
  private readonly string _frontendUrl;

  public ForgotPasswordHandler(
      IRepository<User> userRepo,
      IPasswordResetRepository resetRepo,
      IEmailSender emailSender,
      string frontendUrl)
  {
    _userRepo = userRepo;
    _resetRepo = resetRepo;
    _emailSender = emailSender;
    _frontendUrl = frontendUrl;
  }

  public async Task<ForgotPasswordResult> Handle(ForgotPasswordQuery request)
  {
    var user = await _userRepo
        .FirstOrDefaultAsync(new UserByEmailSpec(request.Email));

    // ❌ If user not found
    if (user == null)
    {
      return new ForgotPasswordResult(
        false,
        "Email address is not registered"
      );
    }

    // ✅ Generate token
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
        $"{_frontendUrl}/reset-password?email={request.Email}&token={tokenValue}";

    var emailBody = $@"
<html>
  <body style='font-family: Arial; background-color:#f4f6f8; padding:20px;'>
    <div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:10px;'>

      <h2>Reset Your Password</h2>

      <p>Hello,</p>

      <p>You requested to reset your password. Click the button below to proceed.</p>

      <div style='text-align:center; margin:30px 0;'>
        <a href='{resetLink}' 
           style='background-color:#ff5722; color:white; padding:12px 20px; text-decoration:none; border-radius:5px;'>
           Reset Password
        </a>
      </div>

      <p>This link will expire in 15 minutes.</p>

      <p>If you didn’t request this, you can ignore this email.</p>

    </div>
  </body>
</html>";

    await _emailSender.SendEmailAsync(
      request.Email,
      "kg834208@gmail.com",
      "Reset Your Password",
      emailBody
    );

    return new ForgotPasswordResult(
      true,
      "Password reset link sent successfully"
    );
  }
}
