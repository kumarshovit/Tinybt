using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public record GetClicksOverTimeQuery(
    DateTime StartDate,
    DateTime EndDate,
    string ViewType
) : IRequest<List<ClickOverTimeDto>>;
