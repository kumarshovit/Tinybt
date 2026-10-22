using System.Text.Json.Serialization;

namespace TinyBtUrlApi.Core.Models;

public class CloudflareScanSubmitResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("result")]
    public CloudflareScanSubmitResult? Result { get; set; }

    [JsonPropertyName("errors")]
    public List<CloudflareApiError>? Errors { get; set; }
}

public class CloudflareScanSubmitResult
{
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class CloudflareScanReportResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("result")]
    public CloudflareScanReportResult? Result { get; set; }

    [JsonPropertyName("errors")]
    public List<CloudflareApiError>? Errors { get; set; }
}

public class CloudflareScanReportResult
{
    [JsonPropertyName("scan")]
    public CloudflareScanDetails? Scan { get; set; }
}

public class CloudflareScanDetails
{
    [JsonPropertyName("verdicts")]
    public CloudflareScanVerdicts? Verdicts { get; set; }

    [JsonPropertyName("page")]
    public CloudflareScanPage? Page { get; set; }
}

public class CloudflareScanVerdicts
{
    [JsonPropertyName("overall")]
    public CloudflareOverallVerdict? Overall { get; set; }
}

public class CloudflareOverallVerdict
{
    [JsonPropertyName("malicious")]
    public bool Malicious { get; set; }

    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }
}

public class CloudflareScanPage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

public class CloudflareApiError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

public class CloudflareScanResult
{
    public bool IsSafe { get; set; } = true;
    public bool IsMalicious { get; set; }
    public string? ScanUuid { get; set; }
    public List<string> Categories { get; set; } = new();
    public string? Description { get; set; }
    public bool TimedOut { get; set; }
    public bool ErrorOccurred { get; set; }
    public bool IsPending { get; set; }

    public static CloudflareScanResult Safe(string? uuid = null) => new() { IsSafe = true, ScanUuid = uuid };
    public static CloudflareScanResult Malicious(string uuid, List<string> categories, string description) =>
        new()
        {
            IsSafe = false,
            IsMalicious = true,
            ScanUuid = uuid,
            Categories = categories,
            Description = description
        };
}
