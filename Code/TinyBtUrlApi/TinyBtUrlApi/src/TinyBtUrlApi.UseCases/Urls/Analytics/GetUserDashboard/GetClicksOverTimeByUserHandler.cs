using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetClicksOverTimeByUserHandler
    : IRequestHandler<GetClicksOverTimeByUserQuery, List<AnalyticsItemDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetClicksOverTimeByUserHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<AnalyticsItemDto>> Handle(
      GetClicksOverTimeByUserQuery request,
      CancellationToken ct)
  {
    return await _repo.GetClicksOverTimeByUserAsync(
        request.UserId,
        request.From,
        request.To,
        request.Link,
    request.Tag,
        ct);
  }
}
