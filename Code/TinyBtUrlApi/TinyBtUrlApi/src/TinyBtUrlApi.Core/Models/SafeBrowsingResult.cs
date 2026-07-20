namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Represents the result of a Google Safe Browsing API check.
/// </summary>
public class SafeBrowsingResult
{
    /// <summary>
    /// Gets a value indicating whether the URL is safe.
    /// </summary>
    public bool IsSafe { get; init; }

    /// <summary>
    /// Gets the type of threat detected, if any.
    /// </summary>
    public string? ThreatType { get; init; }

    /// <summary>
    /// Gets a description or reason for the block.
    /// </summary>
    public string? Description { get; init; }
}
