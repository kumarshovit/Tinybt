using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Entities;

internal class UrlMapping
{

  public int Id { get; set; }
  public string LongUrl { get; set; }
  public string ShortCode { get; set; }
  public int ClickCount { get; set; }
  public DateTime CreatedAt { get; set; }

  public bool IsDeleted { get; set; } = false;
  public DateTime? DeletedAt { get; set; }

  public DateTime? ExpirationDate { get; set; }

  public ICollection<UrlTag> UrlTags { get; set; }
}
