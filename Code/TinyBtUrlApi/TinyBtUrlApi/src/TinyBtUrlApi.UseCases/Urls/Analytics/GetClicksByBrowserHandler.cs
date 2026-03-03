using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetClicksByBrowserHandler(IUrlRepository repository)
    : IRequestHandler<GetClicksByBrowserQuery, List<ClicksByBrowserDto>>
{
  public async ValueTask<List<ClicksByBrowserDto>> Handle(
      GetClicksByBrowserQuery request,
      CancellationToken cancellationToken)
  {
    return await repository.GetClicksByBrowserAsync(
        request.StartDate,
        request.EndDate,
        cancellationToken);
  }
}
