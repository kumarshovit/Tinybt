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
  private readonly IUrlSafetyOrchestrator _safetyOrchestrator;
  private readonly IUrlAbuseTrackingService _abuseTrackingService;

  public CreateShortUrlHandler(
      IUrlRepository repo,
      ShortCodeService shortCodeService,
      ISettingsRepository settingsRepo,
      IUrlSecurityValidator urlSecurityValidator,
      ICaptchaService captchaService,
      IUrlSafetyOrchestrator safetyOrchestrator,
      IUrlAbuseTrackingService abuseTrackingService)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
    _settingsRepo = settingsRepo;
    _urlSecurityValidator = urlSecurityValidator;
    _captchaService = captchaService;
    _safetyOrchestrator = safetyOrchestrator;
    _abuseTrackingService = abuseTrackingService;
  }

  public async ValueTask<CreateShortUrlResult> Handle(
      CreateShortUrlCommand request,
      CancellationToken ct)
  {
    // 🔹 -1. IP Abuse & Malicious Lockout Check (Fails immediately without hitting external APIs or DB)
    if (_abuseTrackingService.IsIpBlocked(request.IpAddress, out var retryAfter, out var blockReason))
    {
      return new CreateShortUrlResult
      {
        Success = false,
        IsBlocked = true,
        RetryAfterSeconds = (int?)retryAfter?.TotalSeconds,
        Message = blockReason ?? $"Your IP address is temporarily blocked due to multiple malicious URL submissions. Please try again in {Math.Ceiling(retryAfter?.TotalMinutes ?? 0)} minute(s)."
      };
    }

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

    // 🔹 1.2 Password validation (length enforced here)
    if (request.Password is not null)
    {
      if (request.Password.Length < 6 || request.Password.Length > 128)
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "Password must be between 6 and 128 characters."
        };
    }

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

    var normalizedUrl = securityResult.NormalizedUrl ?? request.LongUrl;

    // 🔹 1.2 Full Pipeline Validation (Redirect Resolution <= 2 hops + Concurrent Security Checks)
    var pipelineResult = await _safetyOrchestrator.ValidateFullPipelineAsync(normalizedUrl, ct);
    if (!pipelineResult.IsValid)
    {
      if (pipelineResult.IsAbuseAttempt)
      {
        _abuseTrackingService.RecordMaliciousAttempt(
            request.IpAddress,
            pipelineResult.FlaggedUrl ?? normalizedUrl,
            pipelineResult.ThreatSource ?? "ThreatIntelligence",
            pipelineResult.ThreatType ?? "Unsafe");
      }

      return new CreateShortUrlResult
      {
        Success = false,
        Message = pipelineResult.ErrorMessage ?? "The destination URL or a redirect hop has been identified as unsafe."
      };
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
      //var settings = await _settingsRepo.GetAsync();

      //if (settings?.DefaultExpirationDays is int days && days > 0)
      //{
      //  expirationDate = DateTime.UtcNow.AddDays(days);
      //}
      expirationDate = null;
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

    // 🔐 Password Protection
    if (!string.IsNullOrWhiteSpace(request.Password))
    {
      mapping.IsPasswordProtected = true;
      mapping.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
    }

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
      CreatedAt = mapping.CreatedAt,
      IsPasswordProtected = mapping.IsPasswordProtected
    };
  }
}
