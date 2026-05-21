using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.UpdateAlias;

public class UpdateAliasHandler : IRequestHandler<UpdateAliasCommand, UrlDto?>
{
  private readonly IUrlRepository _repo;

  public UpdateAliasHandler(IUrlRepository repo)
  {
    _repo = repo;
  }

  public async ValueTask<UrlDto?> Handle(UpdateAliasCommand request, CancellationToken ct)
  {
    var exists = await _repo.ShortCodeExists(request.NewAlias);
    if (exists) return null;

    var url = await _repo.GetByIdAsync(request.Id);
    if (url == null) return null;

    if (url.UserId != request.UserId && !request.IsAdmin) return null;

    url.ShortCode = request.NewAlias;
    await _repo.UpdateAsync(url);

    return new UrlDto
    {
      Id = url.Id,
      LongUrl = url.LongUrl,
      ShortCode = url.ShortCode,
      ClickCount = url.ClickCount,
      ExpirationDate = url.ExpirationDate
    };
  }
}
