using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

using Mediator;
using TinyBtUrlApi.Core.Interfaces;

public class GetTotalClicksHandler : IRequestHandler<GetTotalClicksQuery, int>
{
  private readonly IUrlRepository _urlRepository;

  public GetTotalClicksHandler(IUrlRepository urlRepository)
  {
    _urlRepository = urlRepository;
  }

  public async ValueTask<int> Handle(GetTotalClicksQuery request, CancellationToken cancellationToken)
  {
    return await _urlRepository.GetTotalClicksAsync(cancellationToken);
  }
}
