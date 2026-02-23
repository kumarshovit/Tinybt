using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.GetAllUrls;

public class GetAllUrlsHandler : IRequestHandler<GetAllUrlsQuery, List<UrlDto>>
{
  private readonly IUrlRepository _repo;

  public GetAllUrlsHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<UrlDto>> Handle(GetAllUrlsQuery request, CancellationToken ct)
  {
    var urls = await _repo.GetAllAsync();

    return urls.Select(u => new UrlDto
    {
      Id = u.Id,
      LongUrl = u.LongUrl,
      ShortCode = u.ShortCode,
      ClickCount = u.ClickCount,
      CreatedAt = u.CreatedAt,
      ExpirationDate = u.ExpirationDate,
      Tags = u.UrlTags.Select(t => t.Tag.Name).ToList()
    }).ToList();

  }
}
