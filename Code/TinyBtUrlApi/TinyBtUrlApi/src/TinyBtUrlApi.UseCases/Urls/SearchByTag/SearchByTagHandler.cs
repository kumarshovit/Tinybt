using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.SearchByTag;

public class SearchByTagHandler : IRequestHandler<SearchByTagQuery, List<UrlDto>>
{
  private readonly IUrlRepository _repo;

  public SearchByTagHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<UrlDto>> Handle(SearchByTagQuery request, CancellationToken ct)
  {
    var urls = await _repo.SearchByTagAsync(request.Tag);

    return urls.Select(u => new UrlDto
    {
      Id = u.Id,
      LongUrl = u.LongUrl,
      ShortCode = u.ShortCode,
      ClickCount = u.ClickCount,
      CreatedAt = u.CreatedAt,
      ExpirationDate = u.ExpirationDate,
      Tags = u.UrlTags?.Select(t => t.Tag.Name).ToList()
    }).ToList();
  }
}
