using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Options;

namespace TinyBtUrlApi.Infrastructure.Security;

/// <summary>
/// Thread-safe in-memory service that tracks confirmed malicious URL attempts per IP address
/// and applies escalating temporary blocks to protect LinkBT from automated abuse.
/// </summary>
public class UrlAbuseTrackingService : IUrlAbuseTrackingService
{
    private readonly IMemoryCache _cache;
    private readonly AbuseProtectionOptions _options;
    private readonly ILogger<UrlAbuseTrackingService> _logger;

    private const string StrikePrefix = "abuse_strike:";
    private const string BlockPrefix = "abuse_block:";
    private const string RepeatOffenderPrefix = "abuse_offender:";

    public UrlAbuseTrackingService(
        IMemoryCache cache,
        IOptions<AbuseProtectionOptions> options,
        ILogger<UrlAbuseTrackingService> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public bool IsIpBlocked(string? ipAddress, out TimeSpan? retryAfter, out string? reason)
    {
        retryAfter = null;
        reason = null;

        if (!_options.Enabled || string.IsNullOrWhiteSpace(ipAddress))
            return false;

        var blockKey = BlockPrefix + ipAddress.Trim();
        if (_cache.TryGetValue(blockKey, out IpBlockInfo? blockInfo) && blockInfo != null)
        {
            var remaining = blockInfo.BlockedUntil - DateTimeOffset.UtcNow;
            if (remaining > TimeSpan.Zero)
            {
                retryAfter = remaining;
                var minutes = Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes));
                reason = $"Too many malicious URL submissions detected from this IP address. Temporarily restricted. Please try again in {minutes} minute(s).";
                return true;
            }

            // Block has elapsed
            _cache.Remove(blockKey);
        }

        return false;
    }

    public void RecordMaliciousAttempt(string? ipAddress, string flaggedUrl, string threatSource, string threatType)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(ipAddress))
            return;

        var cleanIp = ipAddress.Trim();
        var strikeKey = StrikePrefix + cleanIp;
        var window = TimeSpan.FromMinutes(Math.Max(1, _options.DetectionWindowMinutes));

        var strikes = _cache.GetOrCreate(strikeKey, entry =>
        {
            entry.SlidingExpiration = window;
            return new List<DateTimeOffset>();
        }) ?? new List<DateTimeOffset>();

        lock (strikes)
        {
            var now = DateTimeOffset.UtcNow;
            var cutoff = now - window;
            strikes.RemoveAll(t => t < cutoff);
            strikes.Add(now);

            _cache.Set(strikeKey, strikes, window);

            _logger.LogWarning(
                "Malicious URL submission recorded for IP {Ip}. Strike {Count}/{Max}. URL: {Url}, Source: {Source}, Threat: {Threat}",
                cleanIp, strikes.Count, _options.MaxMaliciousAttempts, flaggedUrl, threatSource, threatType);

            if (strikes.Count >= _options.MaxMaliciousAttempts)
            {
                ApplyBlock(cleanIp, strikes.Count);
            }
        }
    }

    private void ApplyBlock(string ipAddress, int strikeCount)
    {
        var offenderKey = RepeatOffenderPrefix + ipAddress;
        var isRepeat = _cache.TryGetValue(offenderKey, out int previousBlockCount);
        var currentBlockCount = isRepeat ? previousBlockCount + 1 : 1;

        // Progressive lockout duration: 15 min for 1st block, 60 min for repeat offenses
        var durationMinutes = currentBlockCount > 1
            ? _options.EscalatedBlockDurationMinutes
            : _options.InitialBlockDurationMinutes;

        var duration = TimeSpan.FromMinutes(Math.Max(1, durationMinutes));
        var blockedUntil = DateTimeOffset.UtcNow.Add(duration);

        var blockInfo = new IpBlockInfo
        {
            IpAddress = ipAddress,
            BlockedAt = DateTimeOffset.UtcNow,
            BlockedUntil = blockedUntil,
            TotalStrikes = strikeCount,
            BlockNumber = currentBlockCount
        };

        // Cache the active block
        _cache.Set(BlockPrefix + ipAddress, blockInfo, duration);

        // Keep repeat offender record in memory for 24 hours to enforce escalated punishment if they resume spamming
        _cache.Set(offenderKey, currentBlockCount, TimeSpan.FromHours(24));

        _logger.LogWarning(
            "🚨 IP {Ip} has been BLOCKED for {Minutes} minutes (Block #{BlockNum}) after {Strikes} malicious submissions. Blocked until: {Until:u}",
            ipAddress, durationMinutes, currentBlockCount, strikeCount, blockedUntil);
    }

    public void UnblockIp(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return;
        var cleanIp = ipAddress.Trim();
        _cache.Remove(BlockPrefix + cleanIp);
        _cache.Remove(StrikePrefix + cleanIp);
        _logger.LogInformation("IP {Ip} has been manually unblocked.", cleanIp);
    }

    public int GetCurrentStrikes(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress)) return 0;
        if (_cache.TryGetValue(StrikePrefix + ipAddress.Trim(), out List<DateTimeOffset>? strikes) && strikes != null)
        {
            lock (strikes)
            {
                var cutoff = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(Math.Max(1, _options.DetectionWindowMinutes));
                return strikes.Count(t => t >= cutoff);
            }
        }
        return 0;
    }

    private class IpBlockInfo
    {
        public string IpAddress { get; set; } = string.Empty;
        public DateTimeOffset BlockedAt { get; set; }
        public DateTimeOffset BlockedUntil { get; set; }
        public int TotalStrikes { get; set; }
        public int BlockNumber { get; set; }
    }
}
