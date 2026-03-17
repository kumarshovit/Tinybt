using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Admin.AdminAnalysis;

public class GetUsersByBrowserHandler
    : IRequestHandler<GetUsersByBrowserQuery, List<UsersByBrowserDto>>
{
  private readonly IAnalyticsRepository _repository;

  public GetUsersByBrowserHandler(IAnalyticsRepository repository)
  {
    _repository = repository;
  }

  public async ValueTask<List<UsersByBrowserDto>> Handle(
      GetUsersByBrowserQuery request,
      CancellationToken ct)
  {
    if (request.StartDate > request.EndDate)
    {
      throw new ArgumentException("Invalid date range");
    }

    var result = await _repository.GetUsersByBrowserAsync(
        request.StartDate,
        request.EndDate,
        ct
    );

    return result;
  }
}
