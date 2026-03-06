using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetDeviceLanguageByUserQuery(
    int UserId,
    DateTime From,
    DateTime To
) : IRequest<List<AnalyticsItemDto>>;
