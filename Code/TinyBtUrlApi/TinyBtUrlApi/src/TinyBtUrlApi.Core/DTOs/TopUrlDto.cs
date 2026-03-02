using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class TopUrlDto
{
  public string ShortCode { get; set; } = default!;
  public string OriginalUrl { get; set; } = default!;
  public int Clicks { get; set; }
}
