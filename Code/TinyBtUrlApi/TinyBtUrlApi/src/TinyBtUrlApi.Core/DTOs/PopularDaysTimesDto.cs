using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class PopularDaysTimesDto
{
  public string Day { get; set; } = default!;
  public int Hour { get; set; }
  public int Clicks { get; set; }
}
