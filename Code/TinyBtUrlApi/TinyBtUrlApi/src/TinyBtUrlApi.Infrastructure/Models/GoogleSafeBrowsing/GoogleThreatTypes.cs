namespace TinyBtUrlApi.Infrastructure.Models.GoogleSafeBrowsing;

/// <summary>
/// Constants for Google Safe Browsing threat types.
/// </summary>
public static class GoogleThreatTypes
{
    public const string Malware = "MALWARE";
    public const string SocialEngineering = "SOCIAL_ENGINEERING";
    public const string UnwantedSoftware = "UNWANTED_SOFTWARE";
    public const string PotentiallyHarmfulApplication = "POTENTIALLY_HARMFUL_APPLICATION";

    /// <summary>
    /// Gets all active threat types to check.
    /// </summary>
    public static string[] All => new[]
    {
        Malware,
        SocialEngineering,
        UnwantedSoftware,
        PotentiallyHarmfulApplication
    };
}
