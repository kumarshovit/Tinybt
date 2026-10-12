namespace TinyBtUrlApi.Core.Models;

public class EmailValidationResult
{
    public bool IsValid { get; set; }
    public bool IsDisposable { get; set; }
    public string? Deliverability { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsProviderAvailable { get; set; } = true;

    public static EmailValidationResult CreateSuccess() =>
        new() { IsValid = true, IsProviderAvailable = true };

    public static EmailValidationResult CreateDisposable(string reason = "Disposable or temporary email addresses are not permitted.") =>
        new() { IsValid = false, IsDisposable = true, RejectionReason = reason, IsProviderAvailable = true };

    public static EmailValidationResult CreateInvalidFormat(string reason = "Invalid email format.") =>
        new() { IsValid = false, RejectionReason = reason, IsProviderAvailable = true };

    public static EmailValidationResult CreateUndeliverable(string reason = "Email address is undeliverable.") =>
        new() { IsValid = false, Deliverability = "UNDELIVERABLE", RejectionReason = reason, IsProviderAvailable = true };

    public static EmailValidationResult CreateRejected(string reason) =>
        new() { IsValid = false, RejectionReason = reason, IsProviderAvailable = true };

    public static EmailValidationResult CreateProviderUnavailable(string reason = "We're temporarily unable to verify this email address. Please try again shortly.") =>
        new() { IsValid = false, IsProviderAvailable = false, RejectionReason = reason };
}
