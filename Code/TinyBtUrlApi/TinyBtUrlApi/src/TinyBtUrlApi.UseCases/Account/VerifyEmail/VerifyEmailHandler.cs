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

    user.IsEmailVerified = true;
    user.EmailVerificationToken = null;

    await _repository.UpdateAsync(user);
    await _repository.SaveChangesAsync();
    return "Email verified successfully. You can now login.";
  }
}
