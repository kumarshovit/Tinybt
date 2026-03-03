using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

public class GetClicksByOsHandler
    : IRequestHandler<GetClicksByOsQuery, List<ClicksByOsDto>>
{
  private readonly IUrlRepository _repository;

  public GetClicksByOsHandler(IUrlRepository repository)
  {
    _repository = repository;
  }

  public async ValueTask<List<ClicksByOsDto>> Handle(
      GetClicksByOsQuery request,
      CancellationToken cancellationToken)
  {
    return await _repository
        .GetClicksByOsAsync(cancellationToken);
  }
}
