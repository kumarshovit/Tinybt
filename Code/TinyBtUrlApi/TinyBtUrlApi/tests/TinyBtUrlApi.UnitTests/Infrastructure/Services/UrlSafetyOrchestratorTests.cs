using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Infrastructure.Services;
using Xunit;

namespace TinyBtUrlApi.UnitTests.Infrastructure.Services;

public class UrlSafetyOrchestratorTests
{
    private readonly IUrlhausService _urlhausService = Substitute.For<IUrlhausService>();
    private readonly IGoogleSafeBrowsingService _safeBrowsingService = Substitute.For<IGoogleSafeBrowsingService>();
    private readonly ICloudflareUrlScannerService _cloudflareScanner = Substitute.For<ICloudflareUrlScannerService>();
    private readonly IUrlRedirectResolver _redirectResolver = Substitute.For<IUrlRedirectResolver>();
    private readonly IUrlRepository _urlRepository = Substitute.For<IUrlRepository>();
    private readonly IOptions<SafetyRecheckOptions> _recheckOptions = Options.Create(new SafetyRecheckOptions());

    private UrlSafetyOrchestrator CreateOrchestrator()
    {
        return new UrlSafetyOrchestrator(
            _urlhausService,
            _safeBrowsingService,
            _cloudflareScanner,
            _redirectResolver,
            _urlRepository,
            _recheckOptions,
            NullLogger<UrlSafetyOrchestrator>.Instance);
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_CleanUrlNoRedirects_ReturnsSuccess()
    {
        // Arrange
        var initialUrl = "https://example.com/clean";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Safe());

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.FinalUrl.ShouldBe(initialUrl);
        result.RedirectChain.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_TwoSafeRedirects_ReturnsSuccess()
    {
        // Arrange: Start -> Hop1 -> Hop2 (2 redirects)
        var initialUrl = "https://start.com";
        var hop1 = "https://hop1.com";
        var hop2 = "https://hop2.com";
        var chain = new List<string> { initialUrl, hop1, hop2 };

        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(hop2, chain));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _cloudflareScanner.QuickCheckUrlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Safe());

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.FinalUrl.ShouldBe(hop2);
        result.RedirectChain.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_RedirectLimitExceeded_FailsFastWithoutCallingSecurityScanners()
    {
        // Arrange
        var initialUrl = "https://spammy-chain.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateFailure(
                "Too many redirects: The URL has exceeded the maximum allowed redirect limit (2 redirects).",
                new List<string> { initialUrl, "https://hop1.com", "https://hop2.com" }));

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("redirect_limit");
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("maximum allowed redirect limit");

        // Verify security scanners were not called when redirect resolution failed
        await _urlhausService.DidNotReceiveWithAnyArgs().CheckUrlsAsync(default!, default);
        await _safeBrowsingService.DidNotReceiveWithAnyArgs().CheckUrlsAsync(default!, default);
        await _cloudflareScanner.DidNotReceiveWithAnyArgs().QuickCheckUrlAsync(default!, default);
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_GoogleSafeBrowsingThreat_FailsValidation()
    {
        // Arrange
        var initialUrl = "https://malware-link.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult
            {
                IsSafe = false,
                ThreatType = "MALWARE",
                FlaggedUrl = initialUrl,
                Description = "Identified as malware by Google Safe Browsing."
            });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Safe());

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("GoogleSafeBrowsing");
        result.ThreatType.ShouldBe("MALWARE");
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_UrlhausThreat_FailsValidation()
    {
        // Arrange
        var initialUrl = "https://phishing.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult
            {
                IsSafe = false,
                ThreatType = "malware_download",
                FlaggedUrl = initialUrl
            });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Safe());

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("URLhaus");
        result.ThreatType.ShouldBe("malware_download");
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_CloudflareThreat_FailsValidation()
    {
        // Arrange
        var initialUrl = "https://malicious.cloud";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Malicious("cf-uuid-999", new List<string> { "Malicious / Phishing" }, "Malicious site detected by Cloudflare"));

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("Cloudflare URL Scanner");
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("Cloudflare");
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_CloudflarePending_FailsClosedNeverFailsOpen()
    {
        // Arrange: Scan is pending and inconclusive
        var initialUrl = "https://slow-scan.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Pending("test-uuid-1234"));

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("pending_or_inconclusive");
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("pending");
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_ProviderThrowsException_FailsClosedNeverFailsOpen()
    {
        // Arrange: Provider service throws an exception
        var initialUrl = "https://error-trigger.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new UrlhausResult { IsSafe = true });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns<Task<SafeBrowsingResult>>(_ => throw new HttpRequestException("SafeBrowsing API down"));

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(CloudflareScanResult.Safe());

        var orchestrator = CreateOrchestrator();

        // Act
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.ThreatSource.ShouldBe("api_error");
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("SafeBrowsing API down");
    }

    [Fact]
    public async Task ValidateFullPipelineAsync_ConcurrentExecution_RunsUrlhausSafeBrowsingAndCloudflareConcurrently()
    {
        // Arrange: Introduce a delay in each provider to verify concurrency via Task.WhenAll
        var initialUrl = "https://concurrency-test.com";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));

        var delay = TimeSpan.FromMilliseconds(80);

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(delay);
                return new UrlhausResult { IsSafe = true };
            });

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(delay);
                return new SafeBrowsingResult { IsSafe = true };
            });

        _cloudflareScanner.QuickCheckUrlAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(async _ =>
            {
                await Task.Delay(delay);
                return CloudflareScanResult.Safe();
            });

        var orchestrator = CreateOrchestrator();

        // Act
        var sw = Stopwatch.StartNew();
        var result = await orchestrator.ValidateFullPipelineAsync(initialUrl);
        sw.Stop();

        // Assert
        result.IsValid.ShouldBeTrue();
        // If sequential: 80ms * 3 = 240ms+. If concurrent: ~80-160ms.
        sw.ElapsedMilliseconds.ShouldBeLessThan(220);
    }
}
