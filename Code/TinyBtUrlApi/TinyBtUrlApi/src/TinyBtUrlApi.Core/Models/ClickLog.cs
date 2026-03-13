using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Models;

public class ClickLog
{
  public int Id { get; set; }
  public string ShortCode { get; set; } = default!;
  public string VisitorId { get; set; } = default!;
  public DateTime ClickedAt { get; set; }
  public string? Referrer { get; set; }
  public string? Country { get; set; }
  public string? DeviceType { get; set; }
  public string? Browser { get; set; }
  public string? OS { get; set; }
  public string? DeviceLanguage { get; set; }
  public string? IpAddress { get; set; }
  public string? RawHeaders { get; set; }
  public string? UserAgent { get; set; }
}
