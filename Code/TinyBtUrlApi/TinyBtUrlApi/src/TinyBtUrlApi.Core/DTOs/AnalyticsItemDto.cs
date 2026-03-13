using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class AnalyticsItemDto
{
  public string Label { get; set; } = string.Empty;

  public int Count { get; set; }
}
