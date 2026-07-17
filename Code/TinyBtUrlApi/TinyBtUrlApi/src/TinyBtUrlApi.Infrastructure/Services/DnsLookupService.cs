using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Services;

public class DnsLookupService : IDnsLookupService
{
    private readonly ILogger<DnsLookupService> _logger;

    public DnsLookupService(ILogger<DnsLookupService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> HasValidDnsRecordsAsync(string host, CancellationToken cancellationToken = default)
    {
        try
        {
            // System.Net.Dns respects the local machine's DNS cache. 
            // Gets IP addresses (IPv4 & IPv6). 
            // Throws SocketException if host cannot be resolved.
            
            var ips = await Dns.GetHostAddressesAsync(host, cancellationToken);
            
            bool hasValidIps = ips != null && ips.Length > 0;
            
            if (!hasValidIps)
            {
                 _logger.LogWarning("DNS lookup returned no IPs for {Host}", host);
                 return false;
            }

            return true;
        }
        catch (SocketException ex)
        {
            _logger.LogWarning(ex, "DNS resolution failed for host {Host}", host);
            return false;
        }
        catch (OperationCanceledException)
        {
            // Throw up to be caught by the caller to log it as a timeout
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error resolving DNS for host {Host}", host);
            return false;
        }
    }
}
