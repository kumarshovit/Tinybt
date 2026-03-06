using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetPopularLinksByUserQuery(
    int UserId,
    DateTime From,
    DateTime To
) : IRequest<List<AnalyticsItemDto>>;
