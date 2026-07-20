using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Service capable of identifying malicious URLs using external APIs.
/// </summary>
public interface IGoogleSafeBrowsingService
{
    /// <summary>
    /// Checks a URL against Google Safe Browsing.
    /// </summary>
    /// <param name="url">The URL to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="SafeBrowsingResult"/> containing the verdict.</returns>
    Task<SafeBrowsingResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default);
}
