using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetAnalyticsQuery(
    int UserId,
    DateTime From,
    DateTime To,
    string Type,
    string? Link,
    string? Tag
) : IRequest<List<AnalyticsItemDto>>;
