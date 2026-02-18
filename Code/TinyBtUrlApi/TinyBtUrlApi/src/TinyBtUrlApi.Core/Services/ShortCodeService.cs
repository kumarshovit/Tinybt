using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Services;

internal class ShortCodeService
{
  public string GenerateShortCode(int length = 6)
  {
    return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        .Replace("/", "_")
        .Replace("+", "-")
        .Substring(0, length);
  }
}
