using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.Analytics;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetClicksByDeviceTypeHandler
    : IRequestHandler<GetClicksByDeviceTypeQuery, List<ClicksByDeviceTypeDto>>
{
  private readonly IUrlRepository _repo;

  public GetClicksByDeviceTypeHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<List<ClicksByDeviceTypeDto>> Handle(
      GetClicksByDeviceTypeQuery request,
      CancellationToken cancellationToken)
  {
    return await _repo.GetClicksByDeviceTypeAsync(
        request.StartDate,
        request.EndDate,
        cancellationToken);
  }
}
