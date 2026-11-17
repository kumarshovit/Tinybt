using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TinyBtUrlApi.Core.Constants;
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
  private readonly IScreenshotQueue _screenshotQueue;

  public CreateShortUrlHandler(
      IUrlRepository repo,
      ShortCodeService shortCodeService,
      ISettingsRepository settingsRepo,
      IUrlSecurityValidator urlSecurityValidator,
      ICaptchaService captchaService,
      IUrlSafetyOrchestrator safetyOrchestrator,
      IUrlAbuseTrackingService abuseTrackingService,
      IScreenshotQueue screenshotQueue)
  {
    _repo = repo;
    _shortCodeService = shortCodeService;
    _settingsRepo = settingsRepo;
    _urlSecurityValidator = urlSecurityValidator;
    _captchaService = captchaService;
    _safetyOrchestrator = safetyOrchestrator;
    _abuseTrackingService = abuseTrackingService;
    _screenshotQueue = screenshotQueue;
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
    bool isCustomAlias = !string.IsNullOrWhiteSpace(request.CustomAlias);

    // 🔹 2. Custom Alias Validation & Reserved Check
    if (isCustomAlias)
    {
      var alias = request.CustomAlias!.Trim().ToLowerInvariant();

      if (ReservedAliases.IsReserved(alias))
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "This alias is reserved and cannot be used."
        };

      if (!Regex.IsMatch(alias, "^[a-zA-Z0-9-]+$"))
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "Alias can contain only letters, numbers and hyphens."
        };

      var exists = await _repo.ShortCodeExists(alias);
      if (exists)
        return new CreateShortUrlResult
        {
          Success = false,
          Message = "Alias already exists."
        };

      shortCode = alias;
    }
    else
    {
      shortCode = string.Empty;
    }

    // 🔹 3. Create Entity Setup
    DateTime? expirationDate = request.ExpirationDate;

    var longUrlHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(normalizedUrl)));
    var isPasswordProtected = !string.IsNullOrWhiteSpace(request.Password);
    string? screenshotPath = null;
    string? scanUuid = null;
    var screenshotStatus = ScreenshotStatus.None;
    DateTime? screenshotFetchedAt = null;

    if (!isPasswordProtected)
    {
      var existingScreenshot = await _repo.GetExistingScreenshotAsync(longUrlHash, ct);
      if (existingScreenshot != null)
      {
        screenshotPath = existingScreenshot.ScreenshotPath;
        scanUuid = existingScreenshot.CloudflareScanUuid;
        screenshotStatus = ScreenshotStatus.Ready;
        screenshotFetchedAt = existingScreenshot.ScreenshotFetchedAt;
      }
      else if (!string.IsNullOrWhiteSpace(pipelineResult.CloudflareScanUuid))
      {
        scanUuid = pipelineResult.CloudflareScanUuid;
        screenshotStatus = ScreenshotStatus.Pending;
      }
    }

    // 🔹 4. Bounded Retry & Safe Unique-Constraint Handling
    const int maxAttempts = 5;
    UrlMapping? savedMapping = null;

    for (int attempt = 1; attempt <= (isCustomAlias ? 1 : maxAttempts); attempt++)
    {
      if (!isCustomAlias)
      {
        int innerAttempts = 0;
        do
        {
          shortCode = _shortCodeService.GenerateShortCode();
          innerAttempts++;
        } while ((ReservedAliases.IsReserved(shortCode) || await _repo.ShortCodeExists(shortCode)) && innerAttempts < 10);
      }

      var mapping = new UrlMapping
      {
        LongUrl = normalizedUrl,
        LongUrlHash = longUrlHash,
        ShortCode = shortCode,
        CreatedAt = DateTime.UtcNow,
        ClickCount = 0,
        ExpirationDate = expirationDate,
        IsDeleted = false,
        UserId = request.UserId,
        IpAddress = request.IpAddress,
        CloudflareScanUuid = scanUuid,
        ScreenshotPath = screenshotPath,
        ScreenshotStatus = screenshotStatus,
        ScreenshotFetchedAt = screenshotFetchedAt
      };

      if (isPasswordProtected)
      {
        mapping.IsPasswordProtected = true;
        mapping.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
      }

      try
      {
        await _repo.AddAsync(mapping);
        savedMapping = mapping;
        break;
      }
      catch (DbUpdateException ex) when (IsShortCodeUniqueViolation(ex))
      {
        _repo.Detach(mapping);

        if (isCustomAlias)
        {
          return new CreateShortUrlResult
          {
            Success = false,
            Message = "Alias already exists."
          };
        }

        if (attempt == maxAttempts)
        {
          return new CreateShortUrlResult
          {
            Success = false,
            Message = "Failed to generate a unique short code after multiple attempts. Please try again."
          };
        }
      }
    }

    if (savedMapping == null)
    {
      return new CreateShortUrlResult
      {
        Success = false,
        Message = "An error occurred while creating the short URL."
      };
    }

    // Enqueue for screenshot download ONLY after SaveChanges (savedMapping.Id exists)
    if (savedMapping.ScreenshotStatus == ScreenshotStatus.Pending)
    {
      await _screenshotQueue.EnqueueAsync(savedMapping.Id, ct);
    }

    // 🔹 5. Return Success Result
    return new CreateShortUrlResult
    {
      Success = true,
      Id = savedMapping.Id,
      ShortCode = savedMapping.ShortCode,
      LongUrl = savedMapping.LongUrl,
      ExpirationDate = savedMapping.ExpirationDate,
      CreatedAt = savedMapping.CreatedAt,
      IsPasswordProtected = savedMapping.IsPasswordProtected
    };
  }

  public static bool IsShortCodeUniqueViolation(DbUpdateException ex)
  {
    for (Exception? current = ex.InnerException; current != null; current = current.InnerException)
    {
      var type = current.GetType();
      // 1. Must originate from SQL Server SqlException
      if (type.FullName == "Microsoft.Data.SqlClient.SqlException" ||
          type.FullName == "System.Data.SqlClient.SqlException" ||
          type.Name == "SqlException")
      {
        // 2. Must have Number property equal to 2601 or 2627
        var numberProp = type.GetProperty("Number");
        if (numberProp?.GetValue(current) is int errorNumber)
        {
          if (errorNumber != 2601 && errorNumber != 2627)
          {
            // Unrelated SQL Server error (e.g. FK 547, deadlock 1205, etc.)
            return false;
          }

          // 3. Must be specifically associated with IX_UrlMappings_ShortCode
          var errorsProp = type.GetProperty("Errors");
          if (errorsProp?.GetValue(current) is System.Collections.IEnumerable errors)
          {
            foreach (var err in errors)
            {
              var errType = err.GetType();
              var errNum = errType.GetProperty("Number")?.GetValue(err) as int?;
              var errMsg = errType.GetProperty("Message")?.GetValue(err) as string ?? "";

              if ((errNum == 2601 || errNum == 2627) &&
                  errMsg.Contains("IX_UrlMappings_ShortCode", StringComparison.OrdinalIgnoreCase))
              {
                return true;
              }
            }
          }

          if (current.Message.Contains("IX_UrlMappings_ShortCode", StringComparison.OrdinalIgnoreCase))
          {
            return true;
          }

          return false;
        }
      }
    }

    return false;
  }
}
