using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Core.Interfaces;

public interface ISettingsRepository
{
  Task<SystemSettings?> GetAsync();
  Task UpdateAsync(SystemSettings settings);
}
