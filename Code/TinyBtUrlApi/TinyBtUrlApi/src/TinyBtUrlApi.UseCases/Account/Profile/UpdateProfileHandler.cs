using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.UseCases.Account.Profile;

public class UpdateProfileHandler
{
  private readonly IRepository<User> _repository;

  public UpdateProfileHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<bool> Handle(UpdateProfileCommand command)
  {
    var spec = new UserByIdSpec(command.UserId);
    var user = await _repository.FirstOrDefaultAsync(spec);

    if (user == null) return false;

    user.FullName = command.FullName;

    await _repository.UpdateAsync(user);

    return true;
  }
}
