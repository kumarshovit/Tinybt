using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.DeleteUrl;

public class DeleteUrlHandler : IRequestHandler<DeleteUrlCommand, bool>
{
  private readonly IUrlRepository _repo;

  public DeleteUrlHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<bool> Handle(DeleteUrlCommand request, CancellationToken ct)
  {
    var url = await _repo.GetByIdAsync(request.Id);
    if (url == null) return false;

    if (url.IsDeleted) return false;

    url.IsDeleted = true;
    url.DeletedAt = DateTime.UtcNow;

    await _repo.UpdateAsync(url);

    return true;
  }
}
