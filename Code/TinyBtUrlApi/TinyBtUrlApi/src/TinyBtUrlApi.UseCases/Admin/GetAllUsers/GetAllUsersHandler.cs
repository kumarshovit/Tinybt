using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Admin.GetAllUsers;

public class GetAllUsersHandler
{
  private readonly IRepository<User> _repo;

  public GetAllUsersHandler(IRepository<User> repo)
  {
    _repo = repo;
  }

  public async Task<List<UserDto>> Handle(GetAllUsersQuery query)
  {
    var users = await _repo.ListAsync();

    return users.Select(u => new UserDto
    {
      Id = u.Id,
      Email = u.Email,
      Role = u.Role,
      IsActive = u.IsActive
    }).ToList();
  }
}
