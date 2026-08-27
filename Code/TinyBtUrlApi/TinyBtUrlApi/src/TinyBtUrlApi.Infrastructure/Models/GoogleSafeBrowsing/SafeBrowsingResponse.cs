using System.Text.Json.Serialization;

namespace TinyBtUrlApi.Infrastructure.Models.GoogleSafeBrowsing;

public class SafeBrowsingResponse
{
    [JsonPropertyName("matches")]
    public SafeBrowsingMatch[]? Matches { get; set; }
}

public class SafeBrowsingMatch
{
    [JsonPropertyName("threatType")]
    public string? ThreatType { get; set; }

    [JsonPropertyName("platformType")]
    public string? PlatformType { get; set; }

    [JsonPropertyName("threatEntryType")]
    public string? ThreatEntryType { get; set; }

    [JsonPropertyName("threat")]
    public SafeBrowsingWarningEntry? Threat { get; set; }
}
