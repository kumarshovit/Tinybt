using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Services;

public class UrlFormatValidator : IUrlFormatValidator
{
  private readonly UrlSecurityOptions _options;

  // 1. Only HTTPS is allowed
  private static readonly HashSet<string> AllowedSchemes = new(StringComparer.OrdinalIgnoreCase)
  {
      Uri.UriSchemeHttps
  };

  // 2. Blocked hostnames or IPs (Localhost, loopback, cloud metadata, production IPs)
  private static readonly HashSet<string> BlockedHosts = new(StringComparer.OrdinalIgnoreCase)
  {
      "localhost",
      "127.0.0.1",
      "::1",
      "0.0.0.0",
      "169.254.169.254", // Cloud Metadata IP (AWS, GCP, Azure SSRF protection)
      "5.230.173.220",   // Production Host IP
      "164.52.216.107"   // Dev / Secondary Host IP
  };

  // 3. Known URL Shorteners (Chaining / Nested redirection prevention)
  private static readonly HashSet<string> BlockedShorteners = new(StringComparer.OrdinalIgnoreCase)
  {
      "bit.ly",
      "tinyurl.com",
      "t.co",
      "cutt.ly",
      "is.gd",
      "ow.ly",
      "rb.gy",
      "shorturl.at",
      "rebrand.ly",
      "tiny.cc",
      "soo.gd",
      "v.gd",
      "qr.ae",
      "buff.ly",
      "adf.ly",
      "link.bt",
      "api.link.bt"
  };

  // 4. Public Object Storage & Cloud Hosting Providers commonly abused for phishing
  private static readonly string[] CloudStorageDomains = new[]
  {
      "storage.googleapis.com",
      "blob.core.windows.net",
      "s3.amazonaws.com",
      "firebaseapp.com",
      "web.app",
      "workers.dev",
      "pages.dev",
      "supabase.co"
  };

  // 5. Script / Web page extensions prohibited on public cloud storage buckets
  private static readonly HashSet<string> CloudStorageRestrictedExtensions = new(StringComparer.OrdinalIgnoreCase)
  {
      ".html",
      ".htm",
      ".xhtml",
      ".php",
      ".asp",
      ".aspx",
      ".jsp",
      ".js",
      ".vbs",
      ".svg"
  };

  // 6. Prohibited executable file downloads
  private static readonly HashSet<string> ProhibitedExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
  {
      ".exe",
      ".scr",
      ".bat",
      ".cmd",
      ".msi",
      ".iso",
      ".vbs",
      ".ps1",
      ".apk"
  };

  public UrlFormatValidator(IOptions<UrlSecurityOptions> options)
  {
    _options = options.Value;
  }

  public UrlSecurityResult ValidateFormat(string url)
  {
    // 1. Empty URL check
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

    // 3. Absolute URL parse
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
    {
      return UrlSecurityResult.CreateFailure("Invalid URL format.");
    }

    // 4. HTTPS only
    if (!AllowedSchemes.Contains(uri.Scheme))
    {
      return UrlSecurityResult.CreateFailure("Only HTTPS URLs are allowed.");
    }

    // 5. Host validation
    if (string.IsNullOrWhiteSpace(uri.Host))
    {
      return UrlSecurityResult.CreateFailure("Hostname is missing.");
    }

    // 6. Block localhost & internal/cloud metadata IPs
    if (BlockedHosts.Contains(uri.Host))
    {
      return UrlSecurityResult.CreateFailure("Localhost, internal addresses, and server host addresses are not allowed.");
    }

    // 7. Block raw IP addresses (e.g., https://192.168.1.1 or https://45.33.32.156)
    if (uri.HostNameType is UriHostNameType.IPv4 or UriHostNameType.IPv6 || IPAddress.TryParse(uri.Host, out _))
    {
      return UrlSecurityResult.CreateFailure("URLs pointing directly to IP addresses are not permitted.");
    }

    // 8. Block Punycode / IDN Homograph domains (e.g., xn--...) used for brand impersonation
    if (uri.Host.StartsWith("xn--", StringComparison.OrdinalIgnoreCase) ||
        uri.Host.Contains(".xn--", StringComparison.OrdinalIgnoreCase))
    {
      return UrlSecurityResult.CreateFailure("Internationalized / Punycode domains are not permitted for security reasons.");
    }

    // 9. Block Chained / Nested URL shorteners (e.g., bit.ly, tinyurl, link.bt)
    if (IsBlockedShortener(uri.Host))
    {
      return UrlSecurityResult.CreateFailure("Shortening links that point to another URL shortener is not permitted.");
    }

    // 10. Block direct executable file downloads (.exe, .scr, .iso, .bat)
    var pathExtension = Path.GetExtension(uri.AbsolutePath);
    if (!string.IsNullOrEmpty(pathExtension) && ProhibitedExecutableExtensions.Contains(pathExtension))
    {
      return UrlSecurityResult.CreateFailure("Direct links to downloadable executable files are not permitted.");
    }

    // 11. Block raw HTML/script files hosted on public cloud storage (Stops Cloud Phishing)
    if (IsCloudStorage(uri.Host) && !string.IsNullOrEmpty(pathExtension) && CloudStorageRestrictedExtensions.Contains(pathExtension))
    {
      return UrlSecurityResult.CreateFailure("Shortening raw HTML or script files hosted on public cloud storage is not permitted.");
    }

    // 12. Reject embedded credentials (e.g., https://user:pass@domain.com)
    if (!string.IsNullOrEmpty(uri.UserInfo))
    {
      return UrlSecurityResult.CreateFailure("URLs containing embedded username/password are not allowed.");
    }

    return UrlSecurityResult.CreateSuccess(uri.ToString());
  }

  private static bool IsBlockedShortener(string host)
  {
    return BlockedShorteners.Contains(host) ||
           BlockedShorteners.Any(shortener => host.EndsWith("." + shortener, StringComparison.OrdinalIgnoreCase));
  }

  private static bool IsCloudStorage(string host)
  {
    return CloudStorageDomains.Any(cloud =>
        host.Equals(cloud, StringComparison.OrdinalIgnoreCase) ||
        host.EndsWith("." + cloud, StringComparison.OrdinalIgnoreCase) ||
        (cloud.Contains("s3.amazonaws.com") && host.Contains(".s3.") && host.EndsWith("amazonaws.com", StringComparison.OrdinalIgnoreCase)));
  }
}
