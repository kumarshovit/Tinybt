using Mediator;
using System.Text.RegularExpressions;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;

namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

public class CreateShortUrlHandler
    : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
  private readonly IUrlRepository _repo;
  private readonly ShortCodeService _shortCodeService;
  private readonly ISettingsRepository _settingsRepo;

  public CreateShortUrlHandler(
      IUrlRepository repo,
      ShortCodeService shortCodeService,
     ISettingsRepository settingsRepo)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
    _settingsRepo = settingsRepo;
  }

  public async ValueTask<CreateShortUrlResult> Handle(
      CreateShortUrlCommand request,
      CancellationToken ct)
  {
    // 🔹 1. Basic Validation

    if (string.IsNullOrWhiteSpace(request.LongUrl))
      return new CreateShortUrlResult
      {
        Success = false,
        Message = "URL is required."
      };

    if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
      return new CreateShortUrlResult
      {
        Success = false,
        Message = "Invalid URL format."
      };

    if (request.ExpirationDate.HasValue &&
        request.ExpirationDate <= DateTime.UtcNow)
      return new CreateShortUrlResult
      {
        Success = false,
        Message = "Expiration date must be in the future."
      };

    string shortCode;

    // 🔹 2. Custom Alias Logic

    if (!string.IsNullOrWhiteSpace(request.CustomAlias))
    {
      shortCode = request.CustomAlias.Trim().ToLower();

      if (!Regex.IsMatch(shortCode, "^[a-zA-Z0-9-]+$"))
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "Alias can contain only letters, numbers and hyphens."
        };

      var exists = await _repo.ShortCodeExists(shortCode);
      if (exists)
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "Alias already exists."
        };
    }
    else
    {
      shortCode = _shortCodeService.GenerateShortCode();
    }

    // 🔹 3.
    // Create Entity

    DateTime? expirationDate = request.ExpirationDate;

    // If user did NOT provide expiration
    if (!expirationDate.HasValue)
    {
      var settings = await _settingsRepo.GetAsync();

      if (settings?.DefaultExpirationDays is int days && days > 0)
      {
        expirationDate = DateTime.UtcNow.AddDays(days);
      }
    }


    var mapping = new UrlMapping
    {
      LongUrl = request.LongUrl,
      ShortCode = shortCode,
      CreatedAt = DateTime.UtcNow,
      ClickCount = 0,
      ExpirationDate = expirationDate,
      IsDeleted = false,
      UserId = request.UserId
    };

    // 🔹 4. Save to DB
    await _repo.AddAsync(mapping);

    // 🔹 5. Return Success Result

    return new CreateShortUrlResult
    {
      Success = true,
      Id = mapping.Id,
      ShortCode = mapping.ShortCode,
      LongUrl = mapping.LongUrl,
      ExpirationDate = mapping.ExpirationDate,
      CreatedAt = mapping.CreatedAt
    };
  }
}
