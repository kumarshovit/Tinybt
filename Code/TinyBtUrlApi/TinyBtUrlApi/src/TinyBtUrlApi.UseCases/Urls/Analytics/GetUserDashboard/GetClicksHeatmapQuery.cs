using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetClicksHeatmapQuery(
    int UserId,
    DateTime Start,
    DateTime End
) : IRequest<List<HeatmapDto>>;
