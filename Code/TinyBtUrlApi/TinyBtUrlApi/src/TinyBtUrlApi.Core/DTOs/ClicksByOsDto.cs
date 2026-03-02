using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class ClicksByOsDto
{
  public string Os { get; set; } = default!;
  public int Clicks { get; set; }
}
