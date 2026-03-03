using System;
using System.Collections.Generic;
using System.Text;

using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetDashboardOverviewQuery : IQuery<DashboardOverviewDto>
{
  public DateTime? From { get; set; }
  public DateTime? To { get; set; }
}
