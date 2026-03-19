using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

public class GetUsersOverTimeHandler
    : IRequestHandler<GetUsersOverTimeQuery, List<UsersOverTimeDto>>
{
  private readonly IAnalyticsRepository _repo;

  public GetUsersOverTimeHandler(IAnalyticsRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<UsersOverTimeDto>> Handle(
      GetUsersOverTimeQuery request,
      CancellationToken ct)
  {
    return await _repo.GetUsersOverTimeAsync(
        request.Start,
        request.End,
        request.ViewType,
        ct);
  }
}
