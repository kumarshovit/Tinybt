using System;
using System.Linq;

namespace TinyBtUrlApi.Core.Services;

public class ShortCodeService
{
  private const string chars =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

  public string GenerateShortCode(int length = 6)
  {
    var random = new Random();

    return new string(
        Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)])
            .ToArray()
    );
  }
}
