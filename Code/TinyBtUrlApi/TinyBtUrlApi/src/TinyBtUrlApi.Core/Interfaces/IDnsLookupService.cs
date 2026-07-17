namespace TinyBtUrlApi.Core.Interfaces;

public interface IDnsLookupService
{
    Task<bool> HasValidDnsRecordsAsync(string host, CancellationToken cancellationToken = default);
}
