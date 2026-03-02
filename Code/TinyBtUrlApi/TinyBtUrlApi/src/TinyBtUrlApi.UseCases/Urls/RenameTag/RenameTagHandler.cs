using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.RenameTag;

public class RenameTagHandler : IRequestHandler<RenameTagCommand>
{
  private readonly IUrlRepository _repo;

  public RenameTagHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<Unit> Handle(RenameTagCommand request, CancellationToken ct)
  {
    await _repo.RenameTagAsync(request.UrlId, request.OldTag, request.NewTag);
    return Unit.Value;
  }
}
