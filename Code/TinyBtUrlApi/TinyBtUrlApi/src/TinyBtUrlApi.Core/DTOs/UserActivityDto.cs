using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class UserActivityDto
{
  public string ActivityType { get; set; } = string.Empty;

  public string? ShortCode { get; set; }

  public string? LongUrl { get; set; }

  public DateTime ActivityTime { get; set; }
}
