using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.UpdateTags;

public class UpdateTagsHandler : IRequestHandler<UpdateTagsCommand>
{
  private readonly IUrlRepository _repo;

  public UpdateTagsHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<Unit> Handle(UpdateTagsCommand request, CancellationToken ct)
  {
    await _repo.UpdateTagsAsync(request.UrlId, request.Tags);
    return Unit.Value;
  }
}
