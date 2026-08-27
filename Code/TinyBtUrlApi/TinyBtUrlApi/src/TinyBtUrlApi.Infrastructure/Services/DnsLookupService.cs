using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Infrastructure.Services;

public class DnsLookupService : IDnsLookupService
{
    private readonly ILogger<DnsLookupService> _logger;

    private static readonly string[] ThreatPatterns = new[]
    {
        ".rpz.",
        "spam",
        "sinkhole",
        "blocklist",
        "airtelspam",
        "malware",
        "phishing"
    };

    public DnsLookupService(ILogger<DnsLookupService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> HasValidDnsRecordsAsync(string host, CancellationToken cancellationToken = default)
    {
        var result = await ValidateDnsAsync(host, cancellationToken);
        return result.IsValid;
    }

    public async Task<DnsValidationResult> ValidateDnsAsync(string host, CancellationToken cancellationToken = default)
    {
        try
        {
            var entry = await Dns.GetHostEntryAsync(host, cancellationToken);

            if (entry == null || entry.AddressList == null || entry.AddressList.Length == 0)
            {
                _logger.LogWarning("DNS lookup returned no IPs for host {Host}", host);
                return DnsValidationResult.Failure($"The domain '{host}' could not be resolved or lacks valid DNS records.");
            }

            var canonical = entry.HostName ?? string.Empty;
            var aliases = entry.Aliases ?? Array.Empty<string>();
            var allNames = new List<string> { canonical }.Concat(aliases);

            // 1. Check if canonical host or alias matches known DNS firewall / RPZ sinkhole markers
            foreach (var name in allNames)
            {
                foreach (var pattern in ThreatPatterns)
                {
                    if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("Host {Host} was sinkholed/flagged by DNS threat intelligence: {CanonicalName}", host, name);
                        return DnsValidationResult.ThreatBlocked(
                            $"The destination domain '{host}' is flagged as malicious/spam or is sinkholed by DNS threat intelligence ({name}).",
                            canonical);
                    }
                }
            }

            // 2. Check if returned IPs are loopback or blackhole IPs
            foreach (var ip in entry.AddressList)
            {
                if (IPAddress.IsLoopback(ip) || ip.Equals(IPAddress.Any) || ip.Equals(IPAddress.IPv6Any))
                {
                    _logger.LogWarning("Host {Host} resolved to loopback/blackhole IP: {IP}", host, ip);
                    return DnsValidationResult.ThreatBlocked(
                        $"The domain '{host}' resolved to an invalid or loopback IP address ({ip}).",
                        canonical);
                }
            }

            return DnsValidationResult.Success(canonical);
        }
        catch (SocketException ex)
        {
            _logger.LogWarning(ex, "DNS resolution failed for host {Host}", host);
            return DnsValidationResult.Failure($"The destination domain '{host}' could not be resolved.");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error resolving DNS for host {Host}", host);
            return DnsValidationResult.Failure($"An error occurred while resolving domain '{host}'.");
        }
    }
}
