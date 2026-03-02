using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;


namespace TinyBtUrlApi.UseCases.Admin.UpdateDefaultExpiration.cs;

public class UpdateDefaultExpirationHandler
    : IRequestHandler<UpdateDefaultExpirationCommand, bool>
{
  private readonly ISettingsRepository _settingsRepo;

  public UpdateDefaultExpirationHandler(ISettingsRepository settingsRepo)
  {
    _settingsRepo = settingsRepo;
  }

  public async ValueTask<bool> Handle(
      UpdateDefaultExpirationCommand request,
      CancellationToken ct)
  {
    var settings = await _settingsRepo.GetAsync();

    if (settings == null)
    {
      settings = new SystemSettings
      {
        DefaultExpirationDays = request.DefaultExpirationDays,
        UpdatedAt = DateTime.UtcNow
      };
    }
    else
    {
      settings.DefaultExpirationDays = request.DefaultExpirationDays;
      settings.UpdatedAt = DateTime.UtcNow;
    }

    await _settingsRepo.UpdateAsync(settings);

    return true;
  }
}
