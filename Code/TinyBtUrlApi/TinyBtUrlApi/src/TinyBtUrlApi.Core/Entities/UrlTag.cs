using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TinyBtUrlApi.Core.Entities;

internal class UrlTag
{
  public int UrlMappingId { get; set; }
  public UrlMapping UrlMapping { get; set; }

  public int TagId { get; set; }
  public Tag Tag { get; set; }
}
