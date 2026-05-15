using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Entities;

public class ContactMessage
{
  public Guid Id { get; set; }

  public string Name { get; set; } = default!;

  public string Email { get; set; } = default!;

  public string Subject { get; set; } = default!;

  public string Message { get; set; } = default!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public bool IsRead { get; set; } = false;
}
