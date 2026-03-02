using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class ClicksByDeviceLanguageDto
{
  public string DeviceLanguage { get; set; } = default!;
  public int Clicks { get; set; }
}
