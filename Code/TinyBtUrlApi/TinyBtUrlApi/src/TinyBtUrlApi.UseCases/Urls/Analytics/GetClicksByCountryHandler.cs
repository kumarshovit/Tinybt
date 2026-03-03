using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetClicksByCountryHandler(IUrlRepository repository)
    : IRequestHandler<GetClicksByCountryQuery, List<ClicksByCountryDto>>
{
  public async ValueTask<List<ClicksByCountryDto>> Handle(
      GetClicksByCountryQuery request,
      CancellationToken cancellationToken)
  {
    return await repository.GetClicksByCountryAsync(
        request.StartDate,
        request.EndDate,
        cancellationToken);
  }
}
