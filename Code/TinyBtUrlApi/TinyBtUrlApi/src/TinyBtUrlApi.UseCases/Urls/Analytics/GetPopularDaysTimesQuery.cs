//using System;
//using System.Collections.Generic;
//using System.Text;
//using Mediator;
//using TinyBtUrlApi.Core.DTOs;

//namespace TinyBtUrlApi.UseCases.Urls.Analytics;

//public record GetPopularDaysTimesQuery(string ShortCode)
//    : IRequest<List<PopularDaysTimesDto>>;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public record GetPopularDaysTimesQuery(
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<List<PopularDaysTimesDto>>;
