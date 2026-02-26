using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Admin.DeleteUser;

public class DeleteUserHandler
{
  private readonly IRepository<User> _repo;

  public DeleteUserHandler(IRepository<User> repo)
  {
    _repo = repo;
  }

  public async Task<string> Handle(DeleteUserCommand command)
  {
    var user = await _repo.GetByIdAsync(command.UserId);

    if (user is null)
      throw new Exception("User not found");

    await _repo.DeleteAsync(user);

    return "User deleted successfully";
  }
}
