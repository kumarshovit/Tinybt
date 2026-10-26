using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public record ContinueRedirectCommand(
    string ShortCode,
    string Token,
    string? UserAgent,
    string? OS,
    string? Country,
    string? DeviceLanguage,
    string? Referrer,
    string? DeviceType,
    string? IpAddress,
    string? RawHeaders
) : IRequest<ContinueRedirectResult>;
