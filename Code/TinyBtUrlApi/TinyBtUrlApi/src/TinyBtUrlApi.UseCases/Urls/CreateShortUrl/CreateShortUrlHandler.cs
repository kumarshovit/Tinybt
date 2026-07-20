using Mediator;
using System.Text.RegularExpressions;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Services;
namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;

public class CreateShortUrlHandler
    : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
  private readonly IUrlRepository _repo;
  private readonly ShortCodeService _shortCodeService;
  private readonly ISettingsRepository _settingsRepo;
  private readonly IUrlSecurityValidator _urlSecurityValidator;
  private readonly ICaptchaService _captchaService;
  private readonly IGoogleSafeBrowsingService _safeBrowsingService;

  public CreateShortUrlHandler(
      IUrlRepository repo,
      ShortCodeService shortCodeService,
      ISettingsRepository settingsRepo,
      IUrlSecurityValidator urlSecurityValidator,
      ICaptchaService captchaService,
      IGoogleSafeBrowsingService safeBrowsingService)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
    _settingsRepo = settingsRepo;
    _urlSecurityValidator = urlSecurityValidator;
    _captchaService = captchaService;
    _safeBrowsingService = safeBrowsingService;
  }

  public async ValueTask<CreateShortUrlResult> Handle(
      CreateShortUrlCommand request,
      CancellationToken ct)
  {
    // 🔹 0. CAPTCHA Validation for unauthenticated users
    if (request.UserId == null)
    {
      var isCaptchaValid = await _captchaService.VerifyTokenAsync(request.CaptchaToken ?? string.Empty, request.IpAddress, ct);
      if (!isCaptchaValid)
      {
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "CAPTCHA verification failed."
        };
      }
    }

    // 🔹 1. Basic Validation
    if (request.ExpirationDate.HasValue &&
        request.ExpirationDate <= DateTime.UtcNow)
      return new CreateShortUrlResult
      {
        Success = false,
        Message = "Expiration date must be in the future."
      };

    // 🔹 1.1 Security Validation & Normalization
    var securityResult = await _urlSecurityValidator.ValidateUrlAsync(request.LongUrl, ct);
    if (!securityResult.Success)
    {
        return new CreateShortUrlResult
        {
            Success = false,
            Message = securityResult.Message
        };
    }

    var normalizedUrl = securityResult.NormalizedUrl;

    // 🔹 4. Google Safe Browsing validation
    if (normalizedUrl != null)
    {
        var safeBrowsingResult = await _safeBrowsingService.CheckUrlAsync(normalizedUrl, ct);
        if (!safeBrowsingResult.IsSafe)
        {
            return new CreateShortUrlResult
            {
                Success = false,
                Message = "The destination URL has been identified as unsafe."
            };
        }
    }

    string shortCode;

    // 🔹 5. Custom Alias Logic

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
      LongUrl = normalizedUrl,
      ShortCode = shortCode,
      CreatedAt = DateTime.UtcNow,
      ClickCount = 0,
      ExpirationDate = expirationDate,
      IsDeleted = false,
      UserId = request.UserId,
      IpAddress = request.IpAddress
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
