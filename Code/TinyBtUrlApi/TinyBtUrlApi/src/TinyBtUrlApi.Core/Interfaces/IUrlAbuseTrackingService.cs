using System;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Tracks confirmed malicious URL submission attempts per IP address
/// and applies progressive lockout penalties to abusive clients.
/// </summary>
public interface IUrlAbuseTrackingService
{
    /// <summary>
    /// Checks whether the given IP address is currently blocked.
    /// </summary>
    bool IsIpBlocked(string? ipAddress, out TimeSpan? retryAfter, out string? reason);

    /// <summary>
    /// Records a confirmed malicious attempt from this IP.
    /// If the threshold is reached, automatically blocks the IP.
    /// </summary>
    void RecordMaliciousAttempt(string? ipAddress, string flaggedUrl, string threatSource, string threatType);

    /// <summary>
    /// Resets / unblocks an IP address manually.
    /// </summary>
    void UnblockIp(string ipAddress);

    /// <summary>
    /// Gets current strike count for an IP in the current window.
    /// </summary>
    int GetCurrentStrikes(string ipAddress);
}
