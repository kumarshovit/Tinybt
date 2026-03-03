using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetTopUrlsHandler(IUrlRepository repository)
    : IRequestHandler<GetTopUrlsQuery, List<TopUrlDto>>
{
  public async ValueTask<List<TopUrlDto>> Handle(
      GetTopUrlsQuery request,
      CancellationToken cancellationToken)
  {
    return await repository.GetTopUrlsAsync(
        request.TopCount,
        cancellationToken);
  }
}
