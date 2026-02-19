using Mediator;

using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;

namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

public class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
  private readonly IUrlRepository _repo;
  private readonly ShortCodeService _shortCodeService;

  public CreateShortUrlHandler(IUrlRepository repo, ShortCodeService shortCodeService)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
  }

  public async ValueTask<CreateShortUrlResult> Handle(CreateShortUrlCommand request, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(request.LongUrl))
      throw new Exception("URL is required.");

    if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
      throw new Exception("Invalid URL format.");

    string shortCode;

    if (!string.IsNullOrWhiteSpace(request.CustomAlias))
    {
      shortCode = request.CustomAlias.Trim().ToLower();

      var exists = await _repo.ShortCodeExists(shortCode);
      if (exists) throw new Exception("Alias already exists.");
    }
    else
    {
      shortCode = _shortCodeService.GenerateShortCode();
    }

    var mapping = new UrlMapping
    {
      LongUrl = request.LongUrl,
      ShortCode = shortCode,
      CreatedAt = DateTime.UtcNow,
      ClickCount = 0,
      ExpirationDate = request.ExpirationDate
    };

    await _repo.AddAsync(mapping);

    return new CreateShortUrlResult(mapping.Id, shortCode);
  }
}
