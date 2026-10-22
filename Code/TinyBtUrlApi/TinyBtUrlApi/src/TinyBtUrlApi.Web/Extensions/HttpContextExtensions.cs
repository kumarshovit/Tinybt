using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TinyBtUrlApi.Web.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Resolves the real client IP address, properly handling Cloudflare (CF-Connecting-IP),
    /// reverse proxies (X-Forwarded-For), and direct connections.
    /// </summary>
    public static string GetClientIpAddress(this HttpContext? context)
    {
        if (context == null) return "127.0.0.1";

        // 1. Cloudflare Connecting IP header (highest priority when behind Cloudflare)
        var cfConnectingIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(cfConnectingIp))
            return cfConnectingIp.Trim();

        // 2. Standard X-Forwarded-For header (first entry is the client IP)
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(xForwardedFor))
        {
            var firstIp = xForwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp))
                return firstIp;
        }

        // 3. Fallback to direct connection IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
    }
}
