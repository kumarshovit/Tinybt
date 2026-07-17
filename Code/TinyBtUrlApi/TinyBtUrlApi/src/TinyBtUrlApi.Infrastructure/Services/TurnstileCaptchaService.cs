using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Services;

public class TurnstileCaptchaService : ICaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private readonly ILogger<TurnstileCaptchaService> _logger;

    public TurnstileCaptchaService(HttpClient httpClient, IConfiguration configuration, ILogger<TurnstileCaptchaService> logger)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Captcha:SecretKey"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<bool> VerifyTokenAsync(string token, string? remoteIp = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("CAPTCHA verification failed: Token is empty.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_secretKey))
        {
            _logger.LogError("CAPTCHA missing configuration: SecretKey is empty.");
            return false;
        }

        var paramList = new List<KeyValuePair<string, string>>
        {
            new("secret", _secretKey),
            new("response", token)
        };
        
        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            paramList.Add(new("remoteip", remoteIp));
        }

        var content = new FormUrlEncodedContent(paramList);

        try
        {
            var response = await _httpClient.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content, ct);
            var responseString = await response.Content.ReadAsStringAsync(ct);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Turnstile API returned non-success status code {StatusCode}. Body: {Body}", response.StatusCode, responseString);
                return false;
            }

            var result = JsonSerializer.Deserialize<TurnstileResponse>(responseString);

            if (result == null || !result.Success)
            {
                var errorCodes = result?.ErrorCodes != null ? string.Join(", ", result.ErrorCodes) : "Unknown";
                _logger.LogWarning("CAPTCHA verification failed. Turnstile Error Codes: {ErrorCodes}", errorCodes);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while verifying the CAPTCHA token.");
            return false;
        }
    }

    private class TurnstileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}
