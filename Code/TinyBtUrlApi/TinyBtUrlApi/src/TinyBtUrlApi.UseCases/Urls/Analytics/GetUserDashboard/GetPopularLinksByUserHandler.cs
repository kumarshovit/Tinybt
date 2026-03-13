using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetPopularLinksByUserHandler
    : IRequestHandler<GetPopularLinksByUserQuery, List<AnalyticsItemDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetPopularLinksByUserHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<AnalyticsItemDto>> Handle(
      GetPopularLinksByUserQuery request,
      CancellationToken ct)
  {
    return await _repo.GetPopularLinksByUserAsync(
        request.UserId,
        request.From,
        request.To,
         request.Link,
    request.Tag,
        ct);
  }
}
