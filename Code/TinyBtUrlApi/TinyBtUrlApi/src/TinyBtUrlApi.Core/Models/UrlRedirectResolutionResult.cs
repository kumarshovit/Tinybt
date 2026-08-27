namespace TinyBtUrlApi.Core.Models;

/// <summary>
/// Represents the result of tracing a URL's redirect chain.
/// </summary>
public class UrlRedirectResolutionResult
{
    public bool Success { get; init; }
    public string? FinalUrl { get; init; }
    public List<string> RedirectChain { get; init; } = new();
    public string? ErrorMessage { get; init; }

    public static UrlRedirectResolutionResult CreateSuccess(string finalUrl, List<string> chain)
    {
        return new UrlRedirectResolutionResult
        {
            Success = true,
            FinalUrl = finalUrl,
            RedirectChain = chain
        };
    }

    public static UrlRedirectResolutionResult CreateFailure(string errorMessage, List<string>? chain = null)
    {
        return new UrlRedirectResolutionResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            RedirectChain = chain ?? new List<string>()
        };
    }
}
