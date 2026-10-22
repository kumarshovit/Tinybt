using Mediator;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public enum RedirectStatus
{
    Found,
    NotFound,
    Expired,
    PasswordProtected,
    Blocked
}

public record RedirectResult(RedirectStatus Status, UrlMapping? Url = null, string? FlaggedReason = null);

public record RedirectUrlQuery(
    string ShortCode,
    string? Browser,
    string? OS,
    string? Country,
    string? DeviceLanguage,
    string? Referrer,
    string? DeviceType,
    string? IpAddress,
    string? RawHeaders,
    string? SourceOverride = null
) : IRequest<RedirectResult>;
