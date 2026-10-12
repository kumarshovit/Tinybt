using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Services;

public class AbstractEmailValidationService : IEmailValidationService
{
    private readonly HttpClient _httpClient;
    private readonly AbstractEmailValidationOptions _options;
    private readonly ILogger<AbstractEmailValidationService> _logger;

    public AbstractEmailValidationService(
        HttpClient httpClient,
        IOptions<AbstractEmailValidationOptions> options,
        ILogger<AbstractEmailValidationService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailValidationResult> ValidateEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return EmailValidationResult.CreateInvalidFormat("Invalid email format.");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var domain = normalizedEmail.Contains('@') ? normalizedEmail.Split('@').Last() : "unknown";

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogError("Abstract Email Validation API key is missing or not configured.");
            return EmailValidationResult.CreateProviderUnavailable();
        }

        var baseUrl = string.IsNullOrWhiteSpace(_options.BaseUrl)
            ? "https://emailvalidation.abstractapi.com/v1/"
            : _options.BaseUrl.TrimEnd('/') + "/";

        var requestUri = $"{baseUrl}?api_key={Uri.EscapeDataString(_options.ApiKey)}&email={Uri.EscapeDataString(normalizedEmail)}";

        try
        {
            var timeoutSeconds = _options.TimeoutSeconds > 0 ? _options.TimeoutSeconds : 3;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            using var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                if (statusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden)
                {
                    _logger.LogError("Abstract Email Validation authentication failed with status code {StatusCode}.", statusCode);
                }
                else if (statusCode == 429)
                {
                    _logger.LogWarning("Abstract Email Validation rate limit or quota exceeded (HTTP 429) for domain {Domain}.", domain);
                }
                else if (statusCode >= 500)
                {
                    _logger.LogError("Abstract Email Validation provider error (HTTP {StatusCode}) for domain {Domain}.", statusCode, domain);
                }
                else
                {
                    _logger.LogWarning("Abstract Email Validation returned unexpected status code {StatusCode} for domain {Domain}.", statusCode, domain);
                }

                return EmailValidationResult.CreateProviderUnavailable();
            }

            using var contentStream = await response.Content.ReadAsStreamAsync(cts.Token);
            using var jsonDoc = await JsonDocument.ParseAsync(contentStream, cancellationToken: cts.Token);
            var root = jsonDoc.RootElement;

            return EvaluateAbstractResponse(root, domain);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Abstract Email Validation timed out after {TimeoutSeconds}s for domain {Domain}.", _options.TimeoutSeconds, domain);
            return EmailValidationResult.CreateProviderUnavailable();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error communicating with Abstract Email Validation for domain {Domain}.", domain);
            return EmailValidationResult.CreateProviderUnavailable();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response from Abstract Email Validation for domain {Domain}.", domain);
            return EmailValidationResult.CreateProviderUnavailable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during email validation for domain {Domain}.", domain);
            return EmailValidationResult.CreateProviderUnavailable();
        }
    }

    private EmailValidationResult EvaluateAbstractResponse(JsonElement root, string domain)
    {
        // 1. Format validation (Email Reputation: email_deliverability.is_format_valid, Legacy: is_valid_format)
        var isValidFormat = TryGetBool(root, "email_deliverability.is_format_valid") ?? TryGetBool(root, "is_valid_format");
        if (isValidFormat == false)
        {
            _logger.LogInformation("Abstract API flagged email format as invalid for domain {Domain}.", domain);
            return EmailValidationResult.CreateInvalidFormat("Invalid email format.");
        }

        // 2. Disposable check (Email Reputation: email_quality.is_disposable, Legacy: is_disposable_email, Flat: is_disposable)
        var isDisposable = TryGetBool(root, "email_quality.is_disposable") 
                           ?? TryGetBool(root, "is_disposable_email") 
                           ?? TryGetBool(root, "is_disposable");
        if (isDisposable == true)
        {
            _logger.LogInformation("Abstract API detected disposable email address for domain {Domain}.", domain);
            return EmailValidationResult.CreateDisposable("Disposable or temporary email addresses are not permitted.");
        }

        // 3. Deliverability check (Email Reputation: email_deliverability.status, Legacy: deliverability)
        var deliverability = TryGetString(root, "email_deliverability.status") ?? TryGetString(root, "deliverability");
        if (!string.IsNullOrWhiteSpace(deliverability) &&
            string.Equals(deliverability, "UNDELIVERABLE", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Abstract API reported deliverability as UNDELIVERABLE for domain {Domain}.", domain);
            return EmailValidationResult.CreateUndeliverable("Email address is undeliverable.");
        }

        // 4. MX records check (Email Reputation: email_deliverability.is_mx_valid, Legacy: is_mx_found)
        var isMxFound = TryGetBool(root, "email_deliverability.is_mx_valid") ?? TryGetBool(root, "is_mx_found");
        if (isMxFound == false)
        {
            _logger.LogInformation("Abstract API reported no MX records found for domain {Domain}.", domain);
            return EmailValidationResult.CreateRejected("Email domain cannot receive mail.");
        }

        // 5. SMTP check (Email Reputation: email_deliverability.is_smtp_valid, Legacy: is_smtp_valid)
        var isSmtpValid = TryGetBool(root, "email_deliverability.is_smtp_valid") ?? TryGetBool(root, "is_smtp_valid");
        if (isSmtpValid == false)
        {
            _logger.LogInformation("Abstract API reported SMTP validation failed for domain {Domain}.", domain);
            return EmailValidationResult.CreateRejected("Email mailbox cannot be reached.");
        }

        return EmailValidationResult.CreateSuccess();
    }

    private static bool? TryGetBool(JsonElement root, string path)
    {
        var current = root;
        var parts = path.Split('.');
        foreach (var part in parts)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current))
                return null;
        }

        if (current.ValueKind == JsonValueKind.True) return true;
        if (current.ValueKind == JsonValueKind.False) return false;

        if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty("value", out var innerValue))
        {
            if (innerValue.ValueKind == JsonValueKind.True) return true;
            if (innerValue.ValueKind == JsonValueKind.False) return false;
            if (innerValue.ValueKind == JsonValueKind.String && bool.TryParse(innerValue.GetString(), out var innerParsed))
                return innerParsed;
        }

        if (current.ValueKind == JsonValueKind.String && bool.TryParse(current.GetString(), out var parsedBool))
        {
            return parsedBool;
        }

        return null;
    }

    private static string? TryGetString(JsonElement root, string path)
    {
        var current = root;
        var parts = path.Split('.');
        foreach (var part in parts)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current))
                return null;
        }

        if (current.ValueKind == JsonValueKind.String)
            return current.GetString();

        return null;
    }
}
