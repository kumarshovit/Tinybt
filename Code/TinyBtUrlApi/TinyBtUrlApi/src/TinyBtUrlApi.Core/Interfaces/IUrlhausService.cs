using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Service for checking URLs against the URLhaus (abuse.ch) malware URL database.
/// </summary>
public interface IUrlhausService
{
    /// <summary>
    /// Checks if a URL is listed in the URLhaus malware database.
    /// </summary>
    /// <param name="url">The URL to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="UrlhausResult"/> containing the verdict.</returns>
    Task<UrlhausResult> CheckUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks multiple URLs against the URLhaus database in a batch (e.g., all redirect chain hops).
    /// </summary>
    /// <param name="urls">The collection of URLs to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="UrlhausResult"/> containing the verdict.</returns>
    Task<UrlhausResult> CheckUrlsAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default);
}
