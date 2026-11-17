using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Mediator;
using TinyBtUrlApi.Core.Constants;
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
    if (string.IsNullOrWhiteSpace(request.NewAlias)) return null;

    var normalizedAlias = request.NewAlias.Trim().ToLowerInvariant();

    if (ReservedAliases.IsReserved(normalizedAlias)) return null;

    if (!Regex.IsMatch(normalizedAlias, "^[a-zA-Z0-9-]+$")) return null;

    var exists = await _repo.ShortCodeExists(normalizedAlias);
    if (exists) return null;

    var url = await _repo.GetByIdAsync(request.Id);
    if (url == null) return null;

    if (url.UserId != request.UserId && !request.IsAdmin) return null;

    url.ShortCode = normalizedAlias;
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
