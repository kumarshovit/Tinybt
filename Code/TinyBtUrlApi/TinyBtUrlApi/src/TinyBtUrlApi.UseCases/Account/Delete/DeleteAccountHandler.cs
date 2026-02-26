using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Delete;

public class DeleteAccountHandler
{
  private readonly IRepository<User> _repository;

  public DeleteAccountHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<bool> Handle(DeleteAccountCommand command)
  {
    var user = await _repository.GetByIdAsync(command.UserId);

    if (user == null)
      return false;

    await _repository.DeleteAsync(user);

    return true;
  }
}
