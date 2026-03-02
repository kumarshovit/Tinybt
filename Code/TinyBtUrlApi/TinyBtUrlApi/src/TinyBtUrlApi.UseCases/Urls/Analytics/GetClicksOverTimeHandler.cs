using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetClicksOverTimeHandler(IUrlRepository repository)
    : IRequestHandler<GetClicksOverTimeQuery, List<ClickOverTimeDto>>
{
  public async ValueTask<List<ClickOverTimeDto>> Handle(
      GetClicksOverTimeQuery request,
      CancellationToken cancellationToken)
  {
    return await repository.GetClicksOverTimeAsync(
        request.StartDate,
        request.EndDate,
        request.ViewType,
        cancellationToken);
  }
}
