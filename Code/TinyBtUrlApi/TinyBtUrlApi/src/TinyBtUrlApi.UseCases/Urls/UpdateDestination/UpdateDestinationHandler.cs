using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.UpdateDestination;

public class UpdateDestinationHandler : IRequestHandler<UpdateDestinationCommand, bool>
{
  private readonly IUrlRepository _repo;

  public UpdateDestinationHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<bool> Handle(UpdateDestinationCommand request, CancellationToken ct)
  {
    var url = await _repo.GetByIdAsync(request.Id);
    if (url == null) return false;

    url.LongUrl = request.NewLongUrl;
    await _repo.UpdateAsync(url);

    return true;
  }
}
