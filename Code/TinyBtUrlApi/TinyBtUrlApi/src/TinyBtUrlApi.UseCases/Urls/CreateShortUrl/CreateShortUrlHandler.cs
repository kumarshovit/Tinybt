using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;


namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

public class CreateShortUrlHandler
  : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
  private readonly IUrlRepository _repo;
  private readonly ShortCodeService _shortCodeService;

  public CreateShortUrlHandler(
      IUrlRepository repo,
      ShortCodeService shortCodeService)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
  }

  public async ValueTask<CreateShortUrlResult> Handle(
      CreateShortUrlCommand request,
      CancellationToken ct)
  {
    // 🔹 Basic Validation
    if (string.IsNullOrWhiteSpace(request.LongUrl))
      throw new ArgumentException("URL is required.");

    if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
      throw new ArgumentException("Invalid URL format.");

    if (request.ExpirationDate.HasValue &&
        request.ExpirationDate <= DateTime.UtcNow)
      throw new ArgumentException("Expiration date must be in the future.");

    string shortCode;

    // 🔹 Custom Alias Logic
    if (!string.IsNullOrWhiteSpace(request.CustomAlias))
    {
      shortCode = request.CustomAlias.Trim().ToLower();

      if (!System.Text.RegularExpressions.Regex.IsMatch(shortCode, "^[a-zA-Z0-9-]+$"))
        throw new ArgumentException("Alias can contain only letters, numbers and hyphens.");

      var exists = await _repo.ShortCodeExists(shortCode);
      if (exists)
        throw new InvalidOperationException("Alias already exists.");
    }
    else
    {
      shortCode = _shortCodeService.GenerateShortCode();
    }

    // 🔹 Create Entity
    var mapping = new UrlMapping
    {
      LongUrl = request.LongUrl,
      ShortCode = shortCode,
      CreatedAt = DateTime.UtcNow,
      ClickCount = 0,
      ExpirationDate = request.ExpirationDate,
      IsDeleted = false
    };

    // 🔹 Save to Database
    await _repo.AddAsync(mapping);

    // 🔹 Return DTO (never return entity)
    return new CreateShortUrlResult(mapping.Id, mapping.ShortCode,
    mapping.LongUrl,
    mapping.ExpirationDate,
    mapping.CreatedAt);
  }
}
