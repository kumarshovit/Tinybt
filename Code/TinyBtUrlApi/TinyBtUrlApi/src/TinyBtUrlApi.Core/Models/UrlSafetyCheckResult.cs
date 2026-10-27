namespace TinyBtUrlApi.Core.Models;

public class UrlSafetyCheckResult
{
    public bool IsSafe { get; init; }
    public string? ThreatSource { get; init; }     // "urlhaus", "google_safe_browsing", "cloudflare"
    public string? ThreatType { get; init; }        // "malware_download", "MALWARE", "SOCIAL_ENGINEERING", etc.
    public string? FlaggedUrl { get; init; }
    public string? Description { get; init; }
    public bool WasApiError { get; init; }          // true if API failed (could not verify)
    public bool IsPending { get; init; }            // true if verification is still pending/queued
    public string? ScanUuid { get; init; }

    public static UrlSafetyCheckResult Safe(string? scanUuid = null) => new() { IsSafe = true, ScanUuid = scanUuid };

    public static UrlSafetyCheckResult Unsafe(string source, string threatType, string flaggedUrl, string description) =>
        new()
        {
            IsSafe = false,
            ThreatSource = source,
            ThreatType = threatType,
            FlaggedUrl = flaggedUrl,
            Description = description
        };

    public static UrlSafetyCheckResult Pending(string source, string description) =>
        new()
        {
            IsSafe = false,
            IsPending = true,
            ThreatSource = source,
            ThreatType = "Pending",
            Description = description
        };

    public static UrlSafetyCheckResult ApiError(string description) =>
        new()
        {
            IsSafe = false,
            WasApiError = true,
            Description = description
        };
}
