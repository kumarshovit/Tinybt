using System;
using System.Collections.Generic;

namespace TinyBtUrlApi.Core.Constants;

public static class ReservedAliases
{
    private static readonly HashSet<string> _reserved = new(StringComparer.OrdinalIgnoreCase)
    {
        // Static and system files
        "robots.txt",
        "sitemap.xml",
        "favicon.ico",
        "google212677b2cefa6fc9.html",
        "index.html",
        "shell.html",
        "sw.js",
        "manifest.json",

        // Public and auth pages
        "login",
        "register",
        "verify-email",
        "forgot-password",
        "reset-password",
        "contact",
        "terms",
        "privacy-policy",
        "privacy",
        "faq",
        "features",
        "pricing",
        "security",

        // Authenticated and system routes
        "dashboard",
        "analytics",
        "developer",
        "profile",
        "admin",
        "tags",
        "blocked-link",
        "expired-link",
        "preview",
        "protected",

        // Reverse proxy and service prefixes
        "api",
        "screenshots",
        "q",

        // Top-level marketing hubs and root SEO slugs
        "tools",
        "use-cases",
        "for",
        "guides",
        "docs",
        "compare",
        "countries",
        "glossary",
        "about",
        "report-abuse",
        "custom-aliases",
        "branded-links",
        "link-expiration",
        "password-protected-links",
        "link-analytics",
        "custom-domains",
        "bulk-shortener",
        "utm-builder",
        "qr-code-generator"
    };

    public static bool IsReserved(string? alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return false;

        return _reserved.Contains(alias.Trim());
    }

    public static IReadOnlyCollection<string> AllReserved => _reserved;
}

