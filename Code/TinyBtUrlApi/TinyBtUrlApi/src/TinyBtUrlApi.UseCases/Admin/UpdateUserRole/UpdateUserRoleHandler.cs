using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Admin.UpdateUserRole;

public class UpdateUserRoleHandler
{
  private readonly IRepository<User> _repo;

  public UpdateUserRoleHandler(IRepository<User> repo)
  {
    _repo = repo;
  }

  public async Task<string> Handle(UpdateUserRoleCommand command)
  {
    var user = await _repo.GetByIdAsync(command.UserId);

    if (user is null)
      throw new Exception("User not found");

    if (command.NewRole != "Admin" && command.NewRole != "User")
      throw new Exception("Invalid role");

    user.Role = command.NewRole;

    await _repo.UpdateAsync(user);

    return "Role updated successfully";
  }
}
