using System;
using System.Collections.Generic;
using System.Text;

//using Mediator;
//using TinyBtUrlApi.Core.DTOs;

//public record GetClicksByDeviceLanguageQuery()
//    : IRequest<List<ClicksByDeviceLanguageDto>>;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

public record GetClicksByDeviceLanguageQuery(
    DateTime? StartDate,
    DateTime? EndDate)
    : IRequest<List<ClicksByDeviceLanguageDto>>;
