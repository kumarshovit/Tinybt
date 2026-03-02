using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class ClickOverTimeDto
{
  public string Period { get; set; } = default!;
  public int Clicks { get; set; }
}
