using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Entities;

public class SystemSettings
{
  public int Id { get; set; }

  // null = no expiration
  public int? DefaultExpirationDays { get; set; }

  public DateTime UpdatedAt { get; set; }
}
