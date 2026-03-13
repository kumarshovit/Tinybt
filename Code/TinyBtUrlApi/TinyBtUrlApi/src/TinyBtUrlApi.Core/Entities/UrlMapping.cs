using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Entities;

public class UrlMapping
{

  public int Id { get; set; }
  public string LongUrl { get; set; } = string.Empty;
  public string ShortCode { get; set; } = string.Empty;
  public int ClickCount { get; set; }
  public DateTime CreatedAt { get; set; }

  public bool IsDeleted { get; set; } = false;
  public DateTime? DeletedAt { get; set; }

  public DateTime? ExpirationDate { get; set; }

  public int? UserId { get; set; }              // 👈 ADD THIS
  public User? User { get; set; } = null!;
  public ICollection<UrlTag> UrlTags { get; set; } = new List<UrlTag>();
}
