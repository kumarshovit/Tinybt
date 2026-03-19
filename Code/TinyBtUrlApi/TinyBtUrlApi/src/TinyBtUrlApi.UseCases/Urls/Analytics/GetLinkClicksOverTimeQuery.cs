using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public class GetLinkClicksOverTimeQuery
    : IRequest<List<ClickOverTimeDto>>
{
  public string ShortCode { get; set; } = default!;
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public string ViewType { get; set; } = "Daily";
  public int? UserId { get; set; }
}
