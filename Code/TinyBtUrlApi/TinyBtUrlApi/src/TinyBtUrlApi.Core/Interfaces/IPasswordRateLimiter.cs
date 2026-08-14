namespace TinyBtUrlApi.Core.Interfaces;

public interface IPasswordRateLimiter
{
    /// <summary>
    /// Checks if the IP has exceeded the maximum allowed failed attempts for the given short code.
    /// </summary>
    bool IsAllowed(string shortCode, string ipAddress);

    /// <summary>
    /// Records a failed password attempt for the given short code and IP.
    /// </summary>
    void RecordFailedAttempt(string shortCode, string ipAddress);
}
