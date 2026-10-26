using System;

namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public enum GetUrlPreviewStatus
{
    Success,
    InvalidToken,
    NotFound,
    Expired,
    Blocked
}

public class GetUrlPreviewResult
{
    public bool Success { get; set; }
    public GetUrlPreviewStatus Status { get; set; }
    public string? ShortCode { get; set; }
    public string? DestinationDomain { get; set; }
    public string? DestinationUrl { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Message { get; set; }

    public static GetUrlPreviewResult Ok(string shortCode, string domain, string destinationUrl, DateTime? expirationDate) =>
        new()
        {
            Success = true,
            Status = GetUrlPreviewStatus.Success,
            ShortCode = shortCode,
            DestinationDomain = domain,
            DestinationUrl = destinationUrl,
            ExpirationDate = expirationDate
        };

    public static GetUrlPreviewResult Invalid(string message = "Invalid or expired preview token.") =>
        new() { Success = false, Status = GetUrlPreviewStatus.InvalidToken, Message = message };

    public static GetUrlPreviewResult NotFound(string message = "Short URL not found.") =>
        new() { Success = false, Status = GetUrlPreviewStatus.NotFound, Message = message };

    public static GetUrlPreviewResult Expired(string message = "This link has expired.") =>
        new() { Success = false, Status = GetUrlPreviewStatus.Expired, Message = message };

    public static GetUrlPreviewResult Blocked(string message = "This URL has been blocked as malicious or unsafe.") =>
        new() { Success = false, Status = GetUrlPreviewStatus.Blocked, Message = message };
}
