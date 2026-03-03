using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

public class GetClicksByDeviceLanguageHandler
    : IRequestHandler<GetClicksByDeviceLanguageQuery, List<ClicksByDeviceLanguageDto>>
{
  private readonly IUrlRepository _repository;

  public GetClicksByDeviceLanguageHandler(IUrlRepository repository)
  {
    _repository = repository;
  }

  public async ValueTask<List<ClicksByDeviceLanguageDto>> Handle(
      GetClicksByDeviceLanguageQuery request,
      CancellationToken cancellationToken)
  {
    return await _repository
        .GetClicksByDeviceLanguageAsync(cancellationToken);
  }
}
