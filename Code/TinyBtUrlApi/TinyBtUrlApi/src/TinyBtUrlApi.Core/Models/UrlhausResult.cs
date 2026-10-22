namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Represents the result of a URLhaus (abuse.ch) malware database check.
/// </summary>
public class UrlhausResult
{
    /// <summary>
    /// Gets a value indicating whether the URL is safe (NOT in the malware database).
    /// </summary>
    public bool IsSafe { get; init; }

    /// <summary>
    /// Gets the type of threat detected, if any (e.g., "malware_download").
    /// </summary>
    public string? ThreatType { get; init; }

    /// <summary>
    /// Gets a human-readable description or reason for the block.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the specific URL in the chain that triggered the threat detection, if any.
    /// </summary>
    public string? FlaggedUrl { get; init; }
}
