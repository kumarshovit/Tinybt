using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class CreateShortUrlHandlerTests
{
    private readonly IUrlRepository _repo = Substitute.For<IUrlRepository>();
    private readonly ShortCodeService _shortCodeService = new();
    private readonly ISettingsRepository _settingsRepo = Substitute.For<ISettingsRepository>();
    private readonly IUrlSecurityValidator _urlSecurityValidator = Substitute.For<IUrlSecurityValidator>();
    private readonly ICaptchaService _captchaService = Substitute.For<ICaptchaService>();
    private readonly IGoogleSafeBrowsingService _safeBrowsingService = Substitute.For<IGoogleSafeBrowsingService>();
    private readonly IUrlRedirectResolver _redirectResolver = Substitute.For<IUrlRedirectResolver>();
    private readonly IUrlhausService _urlhausService = Substitute.For<IUrlhausService>();
    private readonly ICloudflareUrlScannerService _cloudflareScanner = Substitute.For<ICloudflareUrlScannerService>();
    private readonly IUrlAbuseTrackingService _abuseTrackingService = Substitute.For<IUrlAbuseTrackingService>();
    private readonly IScreenshotQueue _screenshotQueue = Substitute.For<IScreenshotQueue>();
    private readonly IUrlSafetyOrchestrator _safetyOrchestrator;

    private readonly CreateShortUrlHandler _handler;

    public CreateShortUrlHandlerTests()
    {
        _safetyOrchestrator = new TinyBtUrlApi.Infrastructure.Services.UrlSafetyOrchestrator(
            _urlhausService,
            _safeBrowsingService,
            _cloudflareScanner,
            _redirectResolver,
            _repo,
            Microsoft.Extensions.Options.Options.Create(new TinyBtUrlApi.Core.Options.SafetyRecheckOptions()),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<TinyBtUrlApi.Infrastructure.Services.UrlSafetyOrchestrator>.Instance);

        _handler = new CreateShortUrlHandler(
            _repo,
            _shortCodeService,
            _settingsRepo,
            _urlSecurityValidator,
            _captchaService,
            _safetyOrchestrator,
            _abuseTrackingService,
            _screenshotQueue);

        TimeSpan? retryAfter = null;
        string? reason = null;
        _abuseTrackingService.IsIpBlocked(Arg.Any<string?>(), out retryAfter, out reason)
            .Returns(false);

        _captchaService.VerifyTokenAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _urlSecurityValidator.ValidateUrlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => UrlSecurityResult.CreateSuccess(callInfo.Arg<string>()));

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new UrlhausResult { IsSafe = true }));

        _cloudflareScanner.QuickCheckUrlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(CloudflareScanResult.Safe()));
    }

    [Fact]
    public async Task Handle_UnsafeHopInRedirectChain_RejectsShortUrlCreation()
    {
        // Arrange: Start -> RedirectHop (unsafe)
        var initialUrl = "https://innocent-looking-site.com/link";
        var unsafeHop = "https://36vj3.speedyconnectedlink.com/?kw=1778";

        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(unsafeHop, new List<string> { initialUrl, unsafeHop }));

        _safeBrowsingService.CheckUrlsAsync(Arg.Is<IEnumerable<string>>(urls => urls.Contains(unsafeHop)), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult
            {
                IsSafe = false,
                ThreatType = "MALWARE",
                FlaggedUrl = unsafeHop,
                Description = $"The destination URL or a redirect hop ('{unsafeHop}') has been identified as unsafe (MALWARE)."
            });

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldNotBeNull();
        result.Message.ShouldContain("unsafe");
        result.Message.ShouldContain(unsafeHop);
    }

    [Fact]
    public async Task Handle_SafeRedirectChain_CreatesShortUrlSuccessfully()
    {
        // Arrange: Start -> SafeLanding
        var initialUrl = "https://shortened-domain.com/landing";
        var safeLanding = "https://actual-destination.com/product";

        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(safeLanding, new List<string> { initialUrl, safeLanding }));

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _repo.AddAsync(Arg.Any<UrlMapping>()).Returns(Task.CompletedTask);

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeTrue();
        result.ShortCode.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_UrlhausUnsafeHop_RejectsShortUrlCreation()
    {
        // Arrange
        var initialUrl = "https://innocent-looking-site.com/link";
        var malwareHop = "https://malware-domain.com/bad.exe";

        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(malwareHop, new List<string> { initialUrl, malwareHop }));

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _urlhausService.CheckUrlsAsync(Arg.Is<IEnumerable<string>>(urls => urls.Contains(malwareHop)), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new UrlhausResult
            {
                IsSafe = false,
                ThreatType = "malware_download",
                FlaggedUrl = malwareHop,
                Description = $"The destination URL or a redirect hop ('{malwareHop}') has been identified as malware by URLhaus (malware_download)."
            }));

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldNotBeNull();
        result.Message.ShouldContain("URLhaus");
        result.Message.ShouldContain(malwareHop);
    }

    [Fact]
    public async Task Handle_MaliciousUrlFlaggedByCloudflare_RejectsShortUrlCreation()
    {
        // Arrange
        var targetUrl = "https://allegro-lokalnie.black951ibg.sbs/";

        _redirectResolver.ResolveChainAsync(targetUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(targetUrl, new List<string> { targetUrl }));

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _urlhausService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new UrlhausResult { IsSafe = true }));

        _cloudflareScanner.QuickCheckUrlAsync(targetUrl, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(CloudflareScanResult.Malicious(
                "test-uuid-123",
                new List<string> { "Phishing" },
                "The destination URL has been identified as malicious by Cloudflare URL Scanner.")));

        var command = new CreateShortUrlCommand(
            LongUrl: targetUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldNotBeNull();
        result.Message.ShouldContain("Cloudflare URL Scanner");
    }

    [Fact]
    public async Task Handle_WhenIpIsBlocked_RejectsImmediatelyWithoutCheckingThreatApis()
    {
        // Arrange
        var testIp = "203.0.113.42";
        TimeSpan? retryAfter = TimeSpan.FromMinutes(12);
        string? reason = "IP blocked due to spam.";

        _abuseTrackingService.IsIpBlocked(testIp, out Arg.Any<TimeSpan?>(), out Arg.Any<string?>())
            .Returns(x =>
            {
                x[1] = retryAfter;
                x[2] = reason;
                return true;
            });

        var command = new CreateShortUrlCommand(
            LongUrl: "https://example.com/safe-page",
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: testIp,
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.IsBlocked.ShouldBeTrue();
        result.RetryAfterSeconds.ShouldBe((int)retryAfter.Value.TotalSeconds);
        result.Message.ShouldContain("IP blocked due to spam.");

        // Verify threat APIs were NOT called (saves quota and latency)
        await _safeBrowsingService.DidNotReceiveWithAnyArgs().CheckUrlsAsync(default!, default);
        await _urlhausService.DidNotReceiveWithAnyArgs().CheckUrlsAsync(default!, default);
    }

    [Fact]
    public async Task Handle_WhenThreatDetected_RecordsMaliciousAttemptForIp()
    {
        // Arrange
        var testIp = "198.51.100.99";
        var maliciousUrl = "https://malware.testing.google.test/malware";

        _redirectResolver.ResolveChainAsync(maliciousUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(maliciousUrl, new List<string> { maliciousUrl }));

        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = false, ThreatType = "MALWARE", FlaggedUrl = maliciousUrl });

        var command = new CreateShortUrlCommand(
            LongUrl: maliciousUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: testIp,
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        _abuseTrackingService.Received(1).RecordMaliciousAttempt(
            testIp,
            maliciousUrl,
            "GoogleSafeBrowsing",
            "MALWARE");
    }
}
