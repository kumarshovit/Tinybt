using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;

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

    // ✅ REAL-WORLD SAFE DELETE (Soft Delete)
    user.IsDeleted = true;
    user.DeletedAt = DateTime.UtcNow;

    await _repository.UpdateAsync(user);

    return true;
  }
}
