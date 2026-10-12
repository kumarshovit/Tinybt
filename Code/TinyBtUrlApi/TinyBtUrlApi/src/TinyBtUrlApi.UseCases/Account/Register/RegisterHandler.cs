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
  private readonly IEmailValidationService _emailValidationService;
  private readonly ICaptchaService? _captchaService;
  private readonly string _frontendUrl;
  private readonly string _smtpFrom;

  public RegisterHandler(
      IRepository<User> repository,
      IEmailSender emailSender,
      IEmailValidationService emailValidationService,
      string frontendUrl,
      string smtpFrom,
      ICaptchaService? captchaService = null)
  {
    _repository = repository;
    _emailSender = emailSender;
    _emailValidationService = emailValidationService;
    _frontendUrl = frontendUrl;
    _smtpFrom = smtpFrom;
    _captchaService = captchaService;
  }

  public static string NormalizeEmail(string email)
  {
    if (string.IsNullOrWhiteSpace(email)) return string.Empty;
    var trimmed = email.Trim().ToLowerInvariant();
    var atIndex = trimmed.IndexOf('@');
    if (atIndex <= 0 || atIndex == trimmed.Length - 1) return trimmed;

    var username = trimmed.Substring(0, atIndex);
    var domain = trimmed.Substring(atIndex + 1);

    if (domain is "gmail.com" or "googlemail.com")
    {
      var plusIndex = username.IndexOf('+');
      if (plusIndex >= 0)
      {
        username = username.Substring(0, plusIndex);
      }

      username = username.Replace(".", "");
    }

    return $"{username}@{domain}";
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

    // ✅ CAPTCHA validation (if enabled)
    if (_captchaService != null)
    {
      var isCaptchaValid = await _captchaService.VerifyTokenAsync(dto.CaptchaToken ?? string.Empty);
      if (!isCaptchaValid)
      {
        return new RegisterResponse
        {
          Success = false,
          Message = "CAPTCHA verification failed."
        };
      }
    }

    var rawEmail = dto.Email.Trim().ToLowerInvariant();
    var normalizedEmail = NormalizeEmail(dto.Email);

    // ✅ Duplicate email check (check normalized first, then raw for backward compatibility)
    var existingUser = await _repository.FirstOrDefaultAsync(
        new UserByEmailSpec(normalizedEmail)
    );

    if (existingUser == null && normalizedEmail != rawEmail)
    {
      existingUser = await _repository.FirstOrDefaultAsync(
          new UserByEmailSpec(rawEmail)
      );
    }

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

      // ✅ If NOT verified → resend verification mail with fresh 24h expiration
      var resendExpiryUnix = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
      var newVerificationToken = $"{Guid.NewGuid():N}.{resendExpiryUnix}";

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

      <p>Please click below to verify your email (link expires in 24 hours).</p>

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

    // ✅ Abstract Email Validation for new registrations
    var validationResult = await _emailValidationService.ValidateEmailAsync(dto.Email);
    if (!validationResult.IsValid)
    {
      return new RegisterResponse
      {
        Success = false,
        Message = validationResult.RejectionReason ?? "We're temporarily unable to verify this email address. Please try again shortly."
      };
    }

    // ✅ Generate verification token with 24-hour expiration (Zero DB schema changes)
    var expiryUnix = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
    var verificationToken = $"{Guid.NewGuid():N}.{expiryUnix}";

    var user = new User
    {
      Email = normalizedEmail,
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

      <p>Thank you for registering. Please verify your email by clicking the button below (link expires in 24 hours).</p>

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


