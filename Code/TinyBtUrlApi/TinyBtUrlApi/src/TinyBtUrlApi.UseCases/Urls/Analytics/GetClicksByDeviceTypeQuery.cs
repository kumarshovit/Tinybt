using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public record GetClicksByDeviceTypeQuery(
    DateTime? StartDate,
    DateTime? EndDate
) : IRequest<List<ClicksByDeviceTypeDto>>;
