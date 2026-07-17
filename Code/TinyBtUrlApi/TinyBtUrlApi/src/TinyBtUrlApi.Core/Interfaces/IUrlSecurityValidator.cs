using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IUrlSecurityValidator
{
    Task<UrlSecurityResult> ValidateUrlAsync(string url, CancellationToken cancellationToken = default);
}
