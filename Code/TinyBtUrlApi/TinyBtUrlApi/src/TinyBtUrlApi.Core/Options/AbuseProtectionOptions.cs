namespace TinyBtUrlApi.Core.Options;

/// <summary>
/// Configuration for progressive IP-based abuse protection against repeated malicious URL submissions.
/// </summary>
public class AbuseProtectionOptions
{
    /// <summary>
    /// Whether the abuse protection tracking is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Max confirmed malicious attempts before an IP is blocked. Default: 4.
    /// </summary>
    public int MaxMaliciousAttempts { get; set; } = 4;

    /// <summary>
    /// Rolling time window (in minutes) to count malicious attempts. Default: 10 minutes.
    /// </summary>
    public int DetectionWindowMinutes { get; set; } = 10;

    /// <summary>
    /// Initial block duration in minutes for first-time offenders. Default: 15 minutes.
    /// </summary>
    public int InitialBlockDurationMinutes { get; set; } = 15;

    /// <summary>
    /// Escalated block duration in minutes for repeat offenders. Default: 60 minutes.
    /// </summary>
    public int EscalatedBlockDurationMinutes { get; set; } = 60;
}
