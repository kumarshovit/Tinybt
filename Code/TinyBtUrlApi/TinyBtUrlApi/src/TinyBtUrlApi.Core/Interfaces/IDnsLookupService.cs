using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IDnsLookupService
{
    Task<bool> HasValidDnsRecordsAsync(string host, CancellationToken cancellationToken = default);
    Task<DnsValidationResult> ValidateDnsAsync(string host, CancellationToken cancellationToken = default);
}
