using System;
using System.Collections.Generic;
using System.Text;
using BCrypt.Net;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Profile;

public class ChangePasswordHandler
{
  private readonly IRepository<User> _repository;

  public ChangePasswordHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<(bool Success, string Message)> Handle(ChangePasswordCommand command)
  {
    var spec = new UserByIdSpec(command.UserId);
    var user = await _repository.FirstOrDefaultAsync(spec);

    if (user == null)
      return (false, "User not found.");

    if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
      return (false, "Current password incorrect.");

    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);

    await _repository.UpdateAsync(user);

    return (true, "Password changed successfully.");
  }
}
