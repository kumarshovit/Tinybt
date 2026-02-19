using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.AddTags;

public class AddTagsHandler : IRequestHandler<AddTagsCommand>
{
  private readonly IUrlRepository _repo;

  public AddTagsHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<Unit> Handle(AddTagsCommand request, CancellationToken ct)
  {
    await _repo.AddTagsAsync(request.UrlId, request.Tags);
    return Unit.Value;
  }
}
