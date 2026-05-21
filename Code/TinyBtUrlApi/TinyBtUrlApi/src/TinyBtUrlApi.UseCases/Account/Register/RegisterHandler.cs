using System.Text.RegularExpressions;
using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Account.Register;

public class RegisterHandler
{
  private readonly IRepository<User> _repository;
  private readonly IEmailSender _emailSender;
  private readonly string _frontendUrl;
  private readonly string _smtpFrom;

  public RegisterHandler(
      IRepository<User> repository,
      IEmailSender emailSender,
      string frontendUrl,
      string smtpFrom)
  {
    _repository = repository;
    _emailSender = emailSender;
    _frontendUrl = frontendUrl;
    _smtpFrom = smtpFrom;
  }

  public async Task<RegisterResponse> Handle(RegisterQuery query)
  {
    var dto = query.Dto;

    // ✅ Email format validation
    if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
      return new RegisterResponse
      {
        Success = false,
        Message = "Invalid email format."
      };

    // ✅ Password strength validation
    if (!Regex.IsMatch(dto.Password,
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$"))
      return new RegisterResponse
      {
        Success = false,
        Message = "Password must be at least 8 characters, include 1 uppercase, 1 lowercase and 1 number."
      };

    // ✅ Duplicate email check
    var existingUser = await _repository.FirstOrDefaultAsync(
        new UserByEmailSpec(dto.Email.ToLower())
    );

  

    if (existingUser != null)
    {
      // ✅ If already verified
      if (existingUser.IsEmailVerified)
      {
        return new RegisterResponse
        {
          Success = false,
          Message = "Email already exists."
        };
      }

      // ✅ If NOT verified → resend verification mail
      var newVerificationToken = Guid.NewGuid().ToString();

      existingUser.EmailVerificationToken = newVerificationToken;

      await _repository.UpdateAsync(existingUser);

      var resendVerificationLink =
          $"{_frontendUrl}/verify-email?token={newVerificationToken}";

      var resendEmailBody = $@"
<html>
<body style='font-family: Arial; background-color:#f4f6f8; padding:20px;'>
<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:10px;'>


      <h2>Verify Your Email</h2>

      <p>Hello,</p>

      <p>Your account is already registered but not verified.</p>

      <p>Please click below to verify your email.</p>

      <div style='text-align:center; margin:30px 0;'>

        <a href='{resendVerificationLink}'
           style='background-color:#4CAF50; color:white; padding:12px 20px; text-decoration:none; border-radius:5px;'>
           Verify Email
        </a>
      </div>

    </div>
  </body>
</html>";

      await _emailSender.SendEmailAsync(
          existingUser.Email,
          _smtpFrom,
          "Verify Your Email",
          resendEmailBody
      );

      return new RegisterResponse
      {
        Success = true,
        Message = "Verification email resent. Please check your inbox."
      };
    }

    // ✅ Generate verification token
    var verificationToken = Guid.NewGuid().ToString();

    var user = new User
    {
      Email = dto.Email.ToLower(),
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
      CreatedAt = DateTime.UtcNow,
      EmailVerificationToken = verificationToken,
      IsEmailVerified = false,
      Role = "User",
      LoginProvider = "Local"
    };

    await _repository.AddAsync(user);

    // ✅ Dynamic frontend URL
    var verificationLink =
        $"{_frontendUrl}/verify-email?token={verificationToken}";

    var emailBody = $@"
<html>
<body style='font-family: Arial; background-color:#f4f6f8; padding:20px;'>
<div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:10px;'>

      <h2>Verify Your Email</h2>

      <p>Hello,</p>

      <p>Thank you for registering. Please verify your email by clicking the button below.</p>

      <div style='text-align:center; margin:30px 0;'>
<a href='{verificationLink}' 
           style='background-color:#4CAF50; color:white; padding:12px 20px; text-decoration:none; border-radius:5px;'>
           Verify Email
</a>
</div>

      <p>If you didn’t create this account, ignore this email.</p>

    </div>
</body>
</html>";

    // ✅ Send email
    await _emailSender.SendEmailAsync(
      user.Email,
      _smtpFrom,
      "Verify Your Email",
      emailBody
    );

    return new RegisterResponse
    {
      Success = true,
      Message = "Registration successful. Please check your email to verify your account."
    };
  }
}


