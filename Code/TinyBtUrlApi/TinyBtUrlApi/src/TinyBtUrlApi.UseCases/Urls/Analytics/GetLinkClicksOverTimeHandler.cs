using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetLinkClicksOverTimeHandler(IUrlRepository repository)
    : IRequestHandler<GetLinkClicksOverTimeQuery, List<ClickOverTimeDto>>
{
  public async ValueTask<List<ClickOverTimeDto>> Handle(
      GetLinkClicksOverTimeQuery request,
      CancellationToken cancellationToken)
  {
    return await repository.GetLinkClicksOverTimeAsync(
        request.ShortCode,
        request.StartDate,
        request.EndDate,
        request.ViewType,
        cancellationToken);
  }
}
