using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.VerifyUrlPassword;

/// <summary>
/// Command sent by the frontend password page.
/// The Source field is securely extracted from the IDataProtector token
/// by the endpoint before calling this handler.
/// </summary>
public record VerifyUrlPasswordCommand(
    string ShortCode,
    string Password,
    string Source,
    // Analytics fields (collected from HTTP headers by the endpoint)
    string? Browser,
    string? OS,
    string? Country,
    string? DeviceLanguage,
    string? Referrer,
    string? DeviceType,
    string? IpAddress,
    string? RawHeaders
) : IRequest<VerifyUrlPasswordResult>;

public record VerifyUrlPasswordResult(
    bool Success,
    string? Message);
