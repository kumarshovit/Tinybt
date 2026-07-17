namespace TinyBtUrlApi.Core.Models;

public class UrlSecurityResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string NormalizedUrl { get; set; } = string.Empty;

    public static UrlSecurityResult CreateSuccess(string normalizedUrl) =>
        new UrlSecurityResult { Success = true, NormalizedUrl = normalizedUrl };
        
    public static UrlSecurityResult CreateFailure(string message) =>
        new UrlSecurityResult { Success = false, Message = message };
}
