using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

public record GetClicksOverTimeByUserQuery(
    int UserId,
    DateTime From,
    DateTime To
) : IRequest<List<AnalyticsItemDto>>;
