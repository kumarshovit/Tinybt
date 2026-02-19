using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TinyBtUrlApi.Core.Entities;

public class UrlTag
{
  public int UrlMappingId { get; set; }
  public UrlMapping UrlMapping { get; set; } = null!;

  public int TagId { get; set; }
  public Tag Tag { get; set; } = null!;
}
