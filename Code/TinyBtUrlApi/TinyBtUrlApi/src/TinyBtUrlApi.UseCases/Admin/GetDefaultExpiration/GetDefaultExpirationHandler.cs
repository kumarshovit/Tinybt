using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Admin.GetDefaultExpiration;

public class GetDefaultExpirationHandler
    : IRequestHandler<GetDefaultExpirationQuery, int?>
{
  private readonly ISettingsRepository _settingsRepo;

  public GetDefaultExpirationHandler(ISettingsRepository settingsRepo)
  {
    _settingsRepo = settingsRepo;
  }

  public async ValueTask<int?> Handle(
      GetDefaultExpirationQuery request,
      CancellationToken ct)
  {
    var settings = await _settingsRepo.GetAsync();
    return settings?.DefaultExpirationDays;
  }
}
