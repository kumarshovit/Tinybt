using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetPopularDaysTimesHandler(IUrlRepository repo)
    : IRequestHandler<GetPopularDaysTimesQuery, List<PopularDaysTimesDto>>
{
  public async ValueTask<List<PopularDaysTimesDto>> Handle(
      GetPopularDaysTimesQuery request,
      CancellationToken ct)
  {
    var clicks = await repo.GetClickLogsByShortCodeAsync(request.ShortCode, ct);

    return clicks
        .GroupBy(x => new
        {
          Day = x.ClickedAt.DayOfWeek,
          Hour = x.ClickedAt.Hour
        })
        .Select(g => new PopularDaysTimesDto
        {
          Day = g.Key.Day.ToString(),
          Hour = g.Key.Hour,
          Clicks = g.Count()
        })
        .OrderBy(x => x.Day)
        .ThenBy(x => x.Hour)
        .ToList();
  }
}
