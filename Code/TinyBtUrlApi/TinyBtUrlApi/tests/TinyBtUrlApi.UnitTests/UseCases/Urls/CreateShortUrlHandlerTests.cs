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

    private readonly CreateShortUrlHandler _handler;

    public CreateShortUrlHandlerTests()
    {
        _handler = new CreateShortUrlHandler(
            _repo,
            _shortCodeService,
            _settingsRepo,
            _urlSecurityValidator,
            _captchaService,
            _safeBrowsingService,
            _redirectResolver);

        _captchaService.VerifyTokenAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _urlSecurityValidator.ValidateUrlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => UrlSecurityResult.CreateSuccess(callInfo.Arg<string>()));
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
}
