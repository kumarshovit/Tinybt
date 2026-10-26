using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.UseCases.Urls.RedirectUrl;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class RedirectUrlHandlerTests
{
    private readonly IUrlRepository _repo = Substitute.For<IUrlRepository>();
    private readonly IUrlSafetyOrchestrator _safetyOrchestrator = Substitute.For<IUrlSafetyOrchestrator>();
    private readonly RedirectUrlHandler _handler;

    public RedirectUrlHandlerTests()
    {
        _handler = new RedirectUrlHandler(_repo, _safetyOrchestrator);

        // Default safety check returns safe
        _safetyOrchestrator.CheckRedirectSafetyAsync(Arg.Any<UrlMapping>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(UrlSafetyCheckResult.Safe()));
    }

    [Fact]
    public async Task Handle_NotFoundOrDeleted_ReturnsNotFound()
    {
        _repo.GetByShortCodeAsync("missing").Returns(Task.FromResult<UrlMapping?>(null));

        var query = new RedirectUrlQuery("missing", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(RedirectStatus.NotFound);
    }

    [Fact]
    public async Task Handle_FlaggedUrl_ReturnsBlockedImmediately()
    {
        var url = new UrlMapping
        {
            Id = 1,
            ShortCode = "danger",
            LongUrl = "https://evil.com",
            IsFlagged = true,
            FlaggedReason = "urlhaus:malware_download"
        };
        _repo.GetByShortCodeAsync("danger").Returns(Task.FromResult<UrlMapping?>(url));

        var query = new RedirectUrlQuery("danger", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(RedirectStatus.Blocked);
        result.FlaggedReason.ShouldBe("urlhaus:malware_download");
        // Orchestrator should not even be called if already flagged
        await _safetyOrchestrator.DidNotReceive().CheckRedirectSafetyAsync(Arg.Any<UrlMapping>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExpiredUrl_ReturnsExpired()
    {
        var url = new UrlMapping
        {
            Id = 2,
            ShortCode = "old",
            LongUrl = "https://example.com",
            ExpirationDate = DateTime.UtcNow.AddMinutes(-5)
        };
        _repo.GetByShortCodeAsync("old").Returns(Task.FromResult<UrlMapping?>(url));

        var query = new RedirectUrlQuery("old", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(RedirectStatus.Expired);
    }

    [Fact]
    public async Task Handle_UnsafeDetectedAtRedirectTime_ReturnsBlocked()
    {
        var url = new UrlMapping
        {
            Id = 3,
            ShortCode = "sneaky",
            LongUrl = "https://compromised.com/payload.exe",
            IsFlagged = false
        };
        _repo.GetByShortCodeAsync("sneaky").Returns(Task.FromResult<UrlMapping?>(url));

        _safetyOrchestrator.CheckRedirectSafetyAsync(url, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(UrlSafetyCheckResult.Unsafe("urlhaus", "malware_download", url.LongUrl, "Malware download detected")));

        var query = new RedirectUrlQuery("sneaky", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(RedirectStatus.Blocked);
        result.FlaggedReason.ShouldContain("Malware download detected");
        // Click should not be logged
        await _repo.DidNotReceive().LogClickAsync(Arg.Any<ClickLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SafeUrl_ReturnsFoundWithoutLoggingClick()
    {
        var url = new UrlMapping
        {
            Id = 4,
            ShortCode = "safe",
            LongUrl = "https://google.com",
            ClickCount = 0
        };
        _repo.GetByShortCodeAsync("safe").Returns(Task.FromResult<UrlMapping?>(url));

        var query = new RedirectUrlQuery("safe", "Chrome", "Windows", "US", "en-US", "https://bing.com", "Desktop", "1.1.1.1", "raw");
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Status.ShouldBe(RedirectStatus.Found);
        // Clicks are now logged in ContinueRedirectHandler when user clicks Continue
        url.ClickCount.ShouldBe(0);
        await _repo.DidNotReceive().LogClickAsync(Arg.Any<ClickLog>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateAsync(url);
    }
}
