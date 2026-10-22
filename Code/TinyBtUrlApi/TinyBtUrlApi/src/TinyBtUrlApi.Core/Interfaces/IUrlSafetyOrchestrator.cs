using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Orchestrates multiple URL safety checks (URLhaus + Google Safe Browsing).
/// Used at redirect-time for quick rechecks and periodic background scanning.
/// </summary>
public interface IUrlSafetyOrchestrator
{
    /// <summary>
    /// Performs a quick safety check on a URL using all available threat intelligence sources.
    /// URLhaus (in-memory lookup, 0ms) is checked first, followed by Google Safe Browsing.
    /// </summary>
    Task<UrlSafetyCheckResult> QuickCheckAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks multiple URLs or redirect hops.
    /// </summary>
    Task<UrlSafetyCheckResult> QuickCheckChainAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluates safety at redirect time, respecting cache window and automatically updating/flagging the URL in repository.
    /// </summary>
    Task<UrlSafetyCheckResult> CheckRedirectSafetyAsync(UrlMapping url, CancellationToken cancellationToken = default);
}
