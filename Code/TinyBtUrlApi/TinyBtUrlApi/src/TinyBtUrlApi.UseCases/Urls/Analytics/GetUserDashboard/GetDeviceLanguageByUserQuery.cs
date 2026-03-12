using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetDeviceLanguageByUserQuery(
    int UserId,
    DateTime From,
    DateTime To,
    string? Link,
    string? Tag
) : IRequest<List<AnalyticsItemDto>>;
