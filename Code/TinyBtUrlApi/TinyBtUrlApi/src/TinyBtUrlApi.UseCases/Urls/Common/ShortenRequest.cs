using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Common;

public class ShortenRequest
{
  public string LongUrl { get; set; } = string.Empty;
  public string? CustomAlias { get; set; }
  public DateTime? ExpirationDate { get; set; }
}
