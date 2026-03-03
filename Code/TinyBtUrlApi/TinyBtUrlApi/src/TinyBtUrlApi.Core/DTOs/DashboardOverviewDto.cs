using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class DashboardOverviewDto
{
  public int TotalClicks { get; set; }
  public int UniqueVisitors { get; set; }
  public int TotalUrls { get; set; }
  public int TotalTags { get; set; }

  public DateTime From { get; set; }
  public DateTime To { get; set; }
}
