using System.Text.Json.Serialization;

namespace TinyBtUrlApi.Infrastructure.Models.GoogleSafeBrowsing;

public class SafeBrowsingRequest
{
    [JsonPropertyName("client")]
    public SafeBrowsingClientInfo? Client { get; set; }

    [JsonPropertyName("threatInfo")]
    public SafeBrowsingThreatInfo? ThreatInfo { get; set; }
}

public class SafeBrowsingClientInfo
{
    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    [JsonPropertyName("clientVersion")]
    public string? ClientVersion { get; set; }
}

public class SafeBrowsingThreatInfo
{
    [JsonPropertyName("threatTypes")]
    public string[]? ThreatTypes { get; set; }

    [JsonPropertyName("platformTypes")]
    public string[]? PlatformTypes { get; set; }

    [JsonPropertyName("threatEntryTypes")]
    public string[]? ThreatEntryTypes { get; set; }

    [JsonPropertyName("threatEntries")]
    public SafeBrowsingWarningEntry[]? ThreatEntries { get; set; }
}

public class SafeBrowsingWarningEntry
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
