using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class ClicksByDeviceTypeDto
{
  public string DeviceType { get; set; } = default!;
  public int Clicks { get; set; }
}
