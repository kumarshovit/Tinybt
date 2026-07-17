
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Services;

public class UrlFormatValidator : IUrlFormatValidator
{
  private readonly UrlSecurityOptions _options;

  // Only HTTPS is allowed
  private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        Uri.UriSchemeHttps
    };

  // Blocked hostnames or IPs
  private static readonly HashSet<string> BlockedHosts = new(StringComparer.OrdinalIgnoreCase)
    {
        "localhost",
        "127.0.0.1",
        "::1",
        "0.0.0.0"
    };

  public UrlFormatValidator(IOptions<UrlSecurityOptions> options)
  {
    _options = options.Value;
  }

  public UrlSecurityResult ValidateFormat(string url)
  {
    // 1. Empty URL
    if (string.IsNullOrWhiteSpace(url))
    {
      return UrlSecurityResult.CreateFailure("URL cannot be empty.");
    }

    // 2. Maximum Length
    if (url.Length > _options.MaxUrlLength)
    {
      return UrlSecurityResult.CreateFailure(
          $"URL exceeds the maximum allowed length of {_options.MaxUrlLength} characters.");
    }

    // 3. Absolute URL
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
    {
      return UrlSecurityResult.CreateFailure("Invalid URL format.");
    }

    // 4. HTTPS only
    if (!AllowedSchemes.Contains(uri.Scheme))
    {
      return UrlSecurityResult.CreateFailure(
          "Only HTTPS URLs are allowed.");
    }

    // 5. Host validation
    if (string.IsNullOrWhiteSpace(uri.Host))
    {
      return UrlSecurityResult.CreateFailure(
          "Hostname is missing.");
    }

    // 6. Block localhost
    if (BlockedHosts.Contains(uri.Host))
    {
      return UrlSecurityResult.CreateFailure(
          "Localhost and loopback addresses are not allowed.");
    }

    // 7. Reject embedded credentials
    if (!string.IsNullOrEmpty(uri.UserInfo))
    {
      return UrlSecurityResult.CreateFailure(
          "URLs containing embedded username/password are not allowed.");
    }

    return UrlSecurityResult.CreateSuccess(uri.ToString());
  }
}
