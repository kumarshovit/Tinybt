using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class ClicksByBrowserDto
{
  public string Browser { get; set; } = default!;
  public int Clicks { get; set; }
}
