using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Interfaces;
using static TinyBtUrlApi.UseCases.Urls.UpdateTagss.UpdareTagsCommand;

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
