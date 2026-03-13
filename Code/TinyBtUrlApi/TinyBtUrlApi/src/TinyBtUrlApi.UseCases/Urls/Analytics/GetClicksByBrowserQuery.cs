using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public record GetClicksByBrowserQuery(
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<List<ClicksByBrowserDto>>;
