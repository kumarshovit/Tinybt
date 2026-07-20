namespace TinyBtUrlApi.Core.Options;

/// <summary>
/// Configuration options for Google Safe Browsing.
/// </summary>
public class GoogleSafeBrowsingOptions
{
    /// <summary>
    /// Gets or sets the API key for Google Safe Browsing v4 endpoint.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
