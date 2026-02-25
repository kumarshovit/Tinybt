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

  public RegisterHandler(
      IRepository<User> repository,
      IEmailSender emailSender)
  {
    _repository = repository;
    _emailSender = emailSender;
  }

  public async Task<string> Handle(RegisterQuery query)
  {
    var dto = query.Dto;

    // ✅ Email format validation
    if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
      return "Invalid email format.";

    // ✅ Password strength validation
    if (!Regex.IsMatch(dto.Password,
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$"))
      return "Password must be at least 8 characters, include 1 uppercase, 1 lowercase and 1 number.";

    // ✅ Duplicate email check
    var existingUser = await _repository.FirstOrDefaultAsync(
        new UserByEmailSpec(dto.Email.ToLower())
    );

    if (existingUser != null)
      return "Email already exists.";

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

    // ✅ Create verification link
    var verificationLink =
        $"http://localhost:5173/verify-email?token={verificationToken}";

    // ✅ Send verification email
    await _emailSender.SendEmailAsync(
    user.Email,
    "kg834208@gmail.com",
    "Verify Your Email",
    $"Click here to verify: {verificationLink}"
);

    return "Registration successful. Please check your email to verify your account.";
  }
}
