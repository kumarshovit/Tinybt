using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.SearchByTag;

public class SearchByTagHandler : IRequestHandler<SearchByTagQuery, List<UrlMapping>>
{
  private readonly IUrlRepository _repo;

  public SearchByTagHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<UrlMapping>> Handle(SearchByTagQuery request, CancellationToken ct)
  {
    return await _repo.SearchByTagAsync(request.Tag);
  }
}
