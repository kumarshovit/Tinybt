using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.DTOs;

// DTOs/UsersByBrowserDto.cs
public class UsersByBrowserDto
{
  public string Browser { get; set; } = default!;
  public int Users { get; set; }
}
