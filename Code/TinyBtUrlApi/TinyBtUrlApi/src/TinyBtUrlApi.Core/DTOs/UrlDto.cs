using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

public class UrlDto
{
  public int Id { get; set; }
  public string LongUrl { get; set; } = default!;
  public string ShortCode { get; set; } = default!;
  public int ClickCount { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? ExpirationDate { get; set; }
  public List<string> Tags { get; set; } = new();
}
