using Mediator;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public record RedirectUrlQuery(
    string ShortCode,
    string? Browser,
    string? OS,
    string? Country,
    string? DeviceLanguage,
    string? Referrer,
    string? DeviceType,
    string? IpAddress,
    string? RawHeaders
) : IRequest<UrlMapping?>;
