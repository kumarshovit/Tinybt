using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Infrastructure.Security;
using Xunit;

namespace TinyBtUrlApi.UnitTests.Infrastructure.Security;

public class UrlAbuseTrackingServiceTests
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly AbuseProtectionOptions _options = new()
    {
        Enabled = true,
        MaxMaliciousAttempts = 4,
        DetectionWindowMinutes = 10,
        InitialBlockDurationMinutes = 15,
        EscalatedBlockDurationMinutes = 60
    };

    private UrlAbuseTrackingService CreateService(AbuseProtectionOptions? options = null)
    {
        return new UrlAbuseTrackingService(
            _cache,
            Options.Create(options ?? _options),
            NullLogger<UrlAbuseTrackingService>.Instance);
    }

    [Fact]
    public void IsIpBlocked_ForNewIp_ReturnsFalse()
    {
        var service = CreateService();
        var blocked = service.IsIpBlocked("192.0.2.1", out var retryAfter, out var reason);

        blocked.ShouldBeFalse();
        retryAfter.ShouldBeNull();
        reason.ShouldBeNull();
    }

    [Fact]
    public void RecordMaliciousAttempt_BelowThreshold_DoesNotBlock()
    {
        var service = CreateService();
        var ip = "192.0.2.2";

        // 3 strikes (threshold is 4)
        service.RecordMaliciousAttempt(ip, "https://malware1.com", "URLhaus", "malware");
        service.RecordMaliciousAttempt(ip, "https://malware2.com", "URLhaus", "malware");
        service.RecordMaliciousAttempt(ip, "https://malware3.com", "URLhaus", "malware");

        service.GetCurrentStrikes(ip).ShouldBe(3);
        service.IsIpBlocked(ip, out _, out _).ShouldBeFalse();
    }

    [Fact]
    public void RecordMaliciousAttempt_ReachingThreshold_BlocksIpFor15Minutes()
    {
        var service = CreateService();
        var ip = "192.0.2.3";

        // 4 strikes reaches threshold
        for (int i = 1; i <= 4; i++)
        {
            service.RecordMaliciousAttempt(ip, $"https://bad-{i}.com", "GoogleSafeBrowsing", "MALWARE");
        }

        var blocked = service.IsIpBlocked(ip, out var retryAfter, out var reason);

        blocked.ShouldBeTrue();
        retryAfter.ShouldNotBeNull();
        retryAfter.Value.TotalMinutes.ShouldBeGreaterThan(14.0);
        retryAfter.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(15.0);
        reason.ShouldNotBeNull();
        reason.ShouldContain("Temporarily restricted");
    }

    [Fact]
    public void RepeatOffender_GetsEscalatedBlockDuration()
    {
        var service = CreateService();
        var ip = "192.0.2.4";

        // 1st block (4 strikes)
        for (int i = 1; i <= 4; i++)
        {
            service.RecordMaliciousAttempt(ip, $"https://malware-{i}.com", "URLhaus", "malware");
        }

        service.IsIpBlocked(ip, out var firstRetry, out _).ShouldBeTrue();
        firstRetry!.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(15.0);

        // Manually simulate unblocking the active block while keeping offender history
        service.UnblockIp(ip);
        service.IsIpBlocked(ip, out _, out _).ShouldBeFalse();

        // 2nd offense within 24 hours: 4 more strikes
        for (int i = 1; i <= 4; i++)
        {
            service.RecordMaliciousAttempt(ip, $"https://malware-repeat-{i}.com", "URLhaus", "malware");
        }

        // Should get escalated block of 60 minutes
        service.IsIpBlocked(ip, out var escalatedRetry, out _).ShouldBeTrue();
        escalatedRetry.ShouldNotBeNull();
        escalatedRetry.Value.TotalMinutes.ShouldBeGreaterThan(50.0);
        escalatedRetry.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(60.0);
    }

    [Fact]
    public void UnblockIp_ManuallyClearsBlockAndStrikes()
    {
        var service = CreateService();
        var ip = "192.0.2.5";

        for (int i = 1; i <= 4; i++)
        {
            service.RecordMaliciousAttempt(ip, "https://threat.com", "Cloudflare", "Phishing");
        }

        service.IsIpBlocked(ip, out _, out _).ShouldBeTrue();

        service.UnblockIp(ip);

        service.IsIpBlocked(ip, out _, out _).ShouldBeFalse();
        service.GetCurrentStrikes(ip).ShouldBe(0);
    }

    [Fact]
    public void WhenDisabled_NeverBlocksIp()
    {
        var disabledOptions = new AbuseProtectionOptions { Enabled = false };
        var service = CreateService(disabledOptions);
        var ip = "192.0.2.6";

        for (int i = 1; i <= 10; i++)
        {
            service.RecordMaliciousAttempt(ip, "https://malware.com", "URLhaus", "malware");
        }

        service.IsIpBlocked(ip, out _, out _).ShouldBeFalse();
    }
}
