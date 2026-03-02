using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public class RedirectUrlHandler : IRequestHandler<RedirectUrlQuery, UrlMapping?>
{
  private readonly IUrlRepository _repo;

  public RedirectUrlHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<UrlMapping?> Handle(RedirectUrlQuery request, CancellationToken ct)
  {
    var url = await _repo.GetByShortCodeAsync(request.ShortCode);

    if (url == null || url.IsDeleted) return null;

    if (url.ExpirationDate.HasValue && url.ExpirationDate < DateTime.UtcNow)
      return null;

    url.ClickCount++;
    await _repo.UpdateAsync(url);

    return url;
  }
}
