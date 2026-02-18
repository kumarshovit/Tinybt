using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Entities;

internal class Tag
{
  public int Id { get; set; }
  public string Name { get; set; }

  public ICollection<UrlTag> UrlTags { get; set; }
}
