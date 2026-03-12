using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public class GetDeviceLanguageByUserHandler
    : IRequestHandler<GetDeviceLanguageByUserQuery, List<AnalyticsItemDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetDeviceLanguageByUserHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<AnalyticsItemDto>> Handle(
      GetDeviceLanguageByUserQuery request,
      CancellationToken ct)
  {
    return await _repo.GetDeviceLanguageByUserAsync(
        request.UserId,
        request.From,
        request.To,
         request.Link,
    request.Tag,
        ct);
  }
}
