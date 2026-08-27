namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Result of validating a domain's DNS records and threat intelligence reputation.
/// </summary>
public class DnsValidationResult
{
    public bool IsValid { get; init; }
    public bool IsThreat { get; init; }
    public string? Reason { get; init; }
    public string? CanonicalHostName { get; init; }

    public static DnsValidationResult Success(string canonicalHostName) => new()
    {
        IsValid = true,
        IsThreat = false,
        CanonicalHostName = canonicalHostName
    };

    public static DnsValidationResult ThreatBlocked(string reason, string canonicalHostName) => new()
    {
        IsValid = false,
        IsThreat = true,
        Reason = reason,
        CanonicalHostName = canonicalHostName
    };

    public static DnsValidationResult Failure(string reason) => new()
    {
        IsValid = false,
        IsThreat = false,
        Reason = reason
    };
}
