namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public enum ContinueRedirectStatus
{
    Success,
    InvalidToken,
    NotFound,
    Expired,
    Blocked
}

public class ContinueRedirectResult
{
    public bool Success { get; set; }
    public ContinueRedirectStatus Status { get; set; }
    public string? RedirectTo { get; set; }
    public string? Message { get; set; }

    public static ContinueRedirectResult Ok(string redirectTo) =>
        new() { Success = true, Status = ContinueRedirectStatus.Success, RedirectTo = redirectTo };

    public static ContinueRedirectResult Invalid(string message = "Invalid or expired token.") =>
        new() { Success = false, Status = ContinueRedirectStatus.InvalidToken, Message = message };

    public static ContinueRedirectResult NotFound(string message = "URL not found.") =>
        new() { Success = false, Status = ContinueRedirectStatus.NotFound, Message = message };

    public static ContinueRedirectResult Expired(string message = "This link has expired.") =>
        new() { Success = false, Status = ContinueRedirectStatus.Expired, Message = message };

    public static ContinueRedirectResult Blocked(string message = "This URL has been blocked as unsafe.") =>
        new() { Success = false, Status = ContinueRedirectStatus.Blocked, Message = message };
}
