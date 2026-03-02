using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.RemoveTag;

public class RemoveTagHandler : IRequestHandler<RemoveTagCommand>
{
  private readonly IUrlRepository _repo;

  public RemoveTagHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<Unit> Handle(RemoveTagCommand request, CancellationToken ct)
  {
    await _repo.RemoveTagAsync(request.UrlId, request.Tag);
    return Unit.Value;
  }
}

