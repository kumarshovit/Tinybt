namespace TinyBtUrlApi.Core.Models;

public class UrlSafetyCheckResult
{
    public bool IsSafe { get; init; }
    public string? ThreatSource { get; init; }     // "urlhaus", "google_safe_browsing", "cloudflare"
    public string? ThreatType { get; init; }        // "malware_download", "MALWARE", "SOCIAL_ENGINEERING", etc.
    public string? FlaggedUrl { get; init; }
    public string? Description { get; init; }
    public bool WasApiError { get; init; }          // true if API failed (could not verify)

    public static UrlSafetyCheckResult Safe() => new() { IsSafe = true };

    public static UrlSafetyCheckResult Unsafe(string source, string threatType, string flaggedUrl, string description) =>
        new()
        {
            IsSafe = false,
            ThreatSource = source,
            ThreatType = threatType,
            FlaggedUrl = flaggedUrl,
            Description = description
        };

    public static UrlSafetyCheckResult ApiError(string description) =>
        new()
        {
            IsSafe = true,
            WasApiError = true,
            Description = description
        };
}
