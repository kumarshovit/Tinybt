using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Admin.DeleteUser;

public class ToggleUserStatusHandler
{
  private readonly IRepository<User> _repo;

  public ToggleUserStatusHandler(IRepository<User> repo)
  {
    _repo = repo;
  }

  public async Task<string> Handle(ToggleUserStatusCommand command)
  {
    var user = await _repo.GetByIdAsync(command.UserId);

    if (user is null)
      throw new Exception("User not found");

    // 🔁 Toggle Active Status
    user.IsActive = !user.IsActive;

    await _repo.UpdateAsync(user);

    return user.IsActive
        ? "User enabled successfully"
        : "User disabled successfully";
  }
}
