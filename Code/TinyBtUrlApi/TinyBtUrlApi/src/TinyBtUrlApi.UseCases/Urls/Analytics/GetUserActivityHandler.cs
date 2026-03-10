using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public sealed class GetUserActivityHandler
    : IQueryHandler<GetUserActivityQuery, List<UserActivityDto>>
{
  private readonly IAnalyticsRepository _analyticsRepository;

  public GetUserActivityHandler(IAnalyticsRepository analyticsRepository)
  {
    _analyticsRepository = analyticsRepository;
  }

  public async ValueTask<List<UserActivityDto>> Handle(
      GetUserActivityQuery query,
      CancellationToken ct)
  {
    return await _analyticsRepository.GetUserActivityAsync(query.UserId, ct);
  }
}
