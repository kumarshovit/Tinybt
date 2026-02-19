using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Entities;

public class Tag
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;

  public ICollection<UrlTag> UrlTags { get; set; } = new List<UrlTag>();
}
