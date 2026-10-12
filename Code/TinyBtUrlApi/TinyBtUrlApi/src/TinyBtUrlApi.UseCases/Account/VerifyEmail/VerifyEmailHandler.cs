using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.VerifyEmail;

public class VerifyEmailHandler
{
  private readonly IRepository<User> _repository;

  public VerifyEmailHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<string> Handle(VerifyEmailQuery query)
  {
    var user = await _repository.FirstOrDefaultAsync(
        new UserByVerificationTokenSpec(query.Token)
    );

    if (user == null)
      return "Invalid or expired token.";

    // Check expiration timestamp (format: <guid>.<unixExpiry>)
    var parts = query.Token.Split('.');
    if (parts.Length == 2 && long.TryParse(parts[1], out var expiryUnix))
    {
      if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiryUnix)
      {
        return "Verification link has expired. Please sign up or request a new verification email.";
      }
    }

    user.IsEmailVerified = true;
    user.EmailVerificationToken = null;

    await _repository.UpdateAsync(user);
    await _repository.SaveChangesAsync();
    return "Email verified successfully. You can now login.";
  }
}
