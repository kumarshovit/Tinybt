using System.Reflection;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Microsoft.EntityFrameworkCore;
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

    [Theory]
    [InlineData("tools")]
    [InlineData("docs")]
    [InlineData("about")]
    [InlineData("use-cases")]
    [InlineData("robots.txt")]
    [InlineData("sitemap.xml")]
    [InlineData("Tools")]
    [InlineData("ABOUT")]
    public async Task Handle_ReservedCustomAlias_RejectsCreation(string reservedSlug)
    {
        // Arrange
        var initialUrl = "https://example.com/safe-page";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: reservedSlug,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("This alias is reserved and cannot be used.");
        await _repo.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<UrlMapping>());
    }

    [Theory]
    [InlineData("invalid alias with space")]
    [InlineData("invalid_with_underscore!")]
    [InlineData("slash/in/alias")]
    public async Task Handle_InvalidCustomAliasFormat_RejectsCreation(string invalidSlug)
    {
        // Arrange
        var initialUrl = "https://example.com/safe-page";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: invalidSlug,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Alias can contain only letters, numbers and hyphens.");
        await _repo.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionUniqueViolation_RetriesAndSucceeds()
    {
        // Arrange
        var initialUrl = "https://example.com/retry-test";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        var attempts = 0;
        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Returns(callInfo =>
            {
                attempts++;
                if (attempts == 1)
                {
                    throw new DbUpdateException(
                        "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'. Cannot insert duplicate key in object 'dbo.UrlMappings'.",
                        CreateSqlException(2627, "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'."));
                }
                return Task.CompletedTask;
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
        result.Success.ShouldBeTrue();
        result.ShortCode.ShouldNotBeNullOrWhiteSpace();
        attempts.ShouldBe(2);
        _repo.Received(1).Detach(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_DbUpdateExceptionUniqueViolation_ExhaustsRetries_ReturnsCleanFailure()
    {
        // Arrange
        var initialUrl = "https://example.com/exhaust-test";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Throws(new DbUpdateException(
                "Cannot insert duplicate key row in object 'dbo.UrlMappings' with unique index 'IX_UrlMappings_ShortCode'.",
                CreateSqlException(2601, "Cannot insert duplicate key row in object 'dbo.UrlMappings' with unique index 'IX_UrlMappings_ShortCode'.")));

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
        result.Message.ShouldBe("Failed to generate a unique short code after multiple attempts. Please try again.");
        _repo.Received(5).Detach(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_CustomAliasDbCollision_ReturnsAliasAlreadyExistsWithoutRetrying()
    {
        // Arrange
        var initialUrl = "https://example.com/custom-collision";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _repo.ShortCodeExists("my-race-alias").Returns(false);

        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Throws(new DbUpdateException(
                "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'.",
                CreateSqlException(2627, "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'.")));

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: "my-race-alias",
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Alias already exists.");
        _repo.Received(1).Detach(Arg.Any<UrlMapping>());
        await _repo.Received(1).AddAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_UnrelatedDbUpdateException_PropagatesException()
    {
        // Arrange
        var initialUrl = "https://example.com/unrelated-error";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        // Unrelated database error (e.g. FK violation 547 or connection break)
        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Throws(new DbUpdateException(
                "The INSERT statement conflicted with the FOREIGN KEY constraint 'FK_UrlMappings_Users_UserId'.",
                new Exception("Foreign key violation")));

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 9999,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act & Assert
        await Should.ThrowAsync<DbUpdateException>(async () =>
        {
            await _handler.Handle(command, CancellationToken.None);
        });

        _repo.DidNotReceive().Detach(Arg.Any<UrlMapping>());
    }

    #region SQL Exception Filter Precision Tests

    [Fact]
    public void IsShortCodeUniqueViolation_Error2601_WithShortCodeIndex_ReturnsTrue()
    {
        var sqlEx = CreateSqlException(2601, "Cannot insert duplicate key row in object 'dbo.UrlMappings' with unique index 'IX_UrlMappings_ShortCode'. The duplicate key value is (abc).");
        var dbEx = new DbUpdateException("Database update error", sqlEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeTrue();
    }

    [Fact]
    public void IsShortCodeUniqueViolation_Error2627_WithShortCodeIndex_ReturnsTrue()
    {
        var sqlEx = CreateSqlException(2627, "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'. Cannot insert duplicate key in object 'dbo.UrlMappings'. The duplicate key value is (abc).");
        var dbEx = new DbUpdateException("Database update error", sqlEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeTrue();
    }

    [Fact]
    public void IsShortCodeUniqueViolation_Error2601_WithAnotherUniqueIndex_ReturnsFalse()
    {
        var sqlEx = CreateSqlException(2601, "Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Email'. The duplicate key value is (test@example.com).");
        var dbEx = new DbUpdateException("Database update error", sqlEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeFalse();
    }

    [Fact]
    public void IsShortCodeUniqueViolation_Error2627_WithAnotherUniqueConstraint_ReturnsFalse()
    {
        var sqlEx = CreateSqlException(2627, "Violation of UNIQUE KEY constraint 'UQ_Users_PhoneNumber'. Cannot insert duplicate key in object 'dbo.Users'.");
        var dbEx = new DbUpdateException("Database update error", sqlEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeFalse();
    }

    [Fact]
    public void IsShortCodeUniqueViolation_UnrelatedSqlError_MentioningShortCodeIndex_ReturnsFalse()
    {
        // For example error 50000 or error 547 with the index name in text/comment
        var sqlEx = CreateSqlException(50000, "Custom stored proc error mentioning IX_UrlMappings_ShortCode in debug trace");
        var dbEx = new DbUpdateException("Database update error", sqlEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeFalse();
    }

    [Fact]
    public void IsShortCodeUniqueViolation_NonSqlServerDbUpdateException_MentioningIndex_ReturnsFalse()
    {
        var innerEx = new InvalidOperationException("Non-SQL exception mentioning IX_UrlMappings_ShortCode in message");
        var dbEx = new DbUpdateException("Database update error", innerEx);

        CreateShortUrlHandler.IsShortCodeUniqueViolation(dbEx).ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WhenSqlError2601Collides_RetriesAndSucceedsOnNextAttempt()
    {
        // Arrange
        var initialUrl = "https://example.com/retry-sql2601";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _repo.ShortCodeExists(Arg.Any<string>()).Returns(false);

        var sqlEx = CreateSqlException(2601, "Cannot insert duplicate key row in object 'dbo.UrlMappings' with unique index 'IX_UrlMappings_ShortCode'.");
        var collisionDbEx = new DbUpdateException("Collision", sqlEx);

        var callCount = 0;
        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Returns(x =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw collisionDbEx;
                }
                var mapping = x.Arg<UrlMapping>();
                mapping.Id = 101;
                return Task.FromResult(mapping);
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
        result.Success.ShouldBeTrue();
        result.ShortCode.ShouldNotBeNullOrWhiteSpace();
        _repo.Received(1).Detach(Arg.Any<UrlMapping>());
        await _repo.Received(2).AddAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_WhenSqlError2627Collides_RetriesAndSucceedsOnNextAttempt()
    {
        // Arrange
        var initialUrl = "https://example.com/retry-sql2627";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        _repo.ShortCodeExists(Arg.Any<string>()).Returns(false);

        var sqlEx = CreateSqlException(2627, "Violation of UNIQUE KEY constraint 'IX_UrlMappings_ShortCode'. Cannot insert duplicate key in object 'dbo.UrlMappings'.");
        var collisionDbEx = new DbUpdateException("Collision", sqlEx);

        var callCount = 0;
        _repo.AddAsync(Arg.Any<UrlMapping>())
            .Returns(x =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw collisionDbEx;
                }
                var mapping = x.Arg<UrlMapping>();
                mapping.Id = 102;
                return Task.FromResult(mapping);
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
        result.Success.ShouldBeTrue();
        result.ShortCode.ShouldNotBeNullOrWhiteSpace();
        _repo.Received(1).Detach(Arg.Any<UrlMapping>());
        await _repo.Received(2).AddAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_WhenSqlError2601OnOtherIndex_DoesNotRetryAndPropagates()
    {
        // Arrange
        var initialUrl = "https://example.com/sql-other-index";
        _redirectResolver.ResolveChainAsync(initialUrl, Arg.Any<CancellationToken>())
            .Returns(UrlRedirectResolutionResult.CreateSuccess(initialUrl, new List<string> { initialUrl }));
        _safeBrowsingService.CheckUrlsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new SafeBrowsingResult { IsSafe = true });

        var sqlEx = CreateSqlException(2601, "Cannot insert duplicate key row in object 'dbo.Users' with unique index 'IX_Users_Email'.");
        var otherIndexDbEx = new DbUpdateException("Other unique index collision", sqlEx);

        _repo.AddAsync(Arg.Any<UrlMapping>()).Throws(otherIndexDbEx);

        var command = new CreateShortUrlCommand(
            LongUrl: initialUrl,
            CustomAlias: null,
            ExpirationDate: null,
            UserId: 1,
            IpAddress: "1.2.3.4",
            CaptchaToken: null,
            Password: null);

        // Act & Assert
        await Should.ThrowAsync<DbUpdateException>(async () =>
        {
            await _handler.Handle(command, CancellationToken.None);
        });

        _repo.DidNotReceive().Detach(Arg.Any<UrlMapping>());
        await _repo.Received(1).AddAsync(Arg.Any<UrlMapping>());
    }

    private static SqlException CreateSqlException(int number, string message)
    {
        var collectionCtor = typeof(SqlErrorCollection).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
        var errorCollection = (SqlErrorCollection)collectionCtor!.Invoke(null);

        var errorCtor = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (errorCtor != null)
        {
            var prms = errorCtor.GetParameters();
            var args = new object?[prms.Length];
            for (int i = 0; i < prms.Length; i++)
            {
                var pType = prms[i].ParameterType;
                if (pType == typeof(int)) args[i] = number;
                else if (pType == typeof(byte)) args[i] = (byte)1;
                else if (pType == typeof(string)) args[i] = message;
                else if (pType == typeof(uint)) args[i] = (uint)0;
                else args[i] = null;
            }
            var error = errorCtor.Invoke(args);

            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
            addMethod?.Invoke(errorCollection, new[] { error });
        }

        var createExceptionMethod = typeof(SqlException).GetMethod(
            "CreateException",
            BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(SqlErrorCollection), typeof(string) },
            null);

        if (createExceptionMethod != null)
        {
            return (SqlException)createExceptionMethod.Invoke(null, new object[] { errorCollection, "16.0.1000" })!;
        }

        var sqlEx = (SqlException)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        typeof(SqlException).GetField("_errors", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(sqlEx, errorCollection);
        typeof(SqlException).GetField("_number", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(sqlEx, number);
        return sqlEx;
    }

    #endregion
}
