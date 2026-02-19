using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.GetAllUrls;

public class GetAllUrlsHandler : IRequestHandler<GetAllUrlsQuery, List<UrlMapping>>
{
  private readonly IUrlRepository _repo;

  public GetAllUrlsHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<UrlMapping>> Handle(GetAllUrlsQuery request, CancellationToken ct)
  {
    return await _repo.GetAllAsync();
  }
}
