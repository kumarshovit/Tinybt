using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

/// <summary>
/// Service responsible for resolving and unwrapping multi-hop HTTP redirects.
/// </summary>
public interface IUrlRedirectResolver
{
    /// <summary>
    /// Follows redirect hops starting from initialUrl and returns all URLs in the chain and final destination.
    /// </summary>
    /// <param name="initialUrl">The starting URL to resolve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="UrlRedirectResolutionResult"/> with all intermediate hops and final landing URL.</returns>
    Task<UrlRedirectResolutionResult> ResolveChainAsync(string initialUrl, CancellationToken cancellationToken = default);
}
