using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.UseCases.Urls.PreviewUrl;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class GetUrlPreviewHandlerTests
{
    private readonly IUrlRepository _repo = Substitute.For<IUrlRepository>();
    private readonly IUrlAccessTokenService _tokenService = Substitute.For<IUrlAccessTokenService>();
    private readonly IUrlSafetyOrchestrator _safetyOrchestrator = Substitute.For<IUrlSafetyOrchestrator>();
    private readonly GetUrlPreviewHandler _handler;

    public GetUrlPreviewHandlerTests()
    {
        _handler = new GetUrlPreviewHandler(_repo, _tokenService, _safetyOrchestrator);

        // Default safety check returns safe
        _safetyOrchestrator.CheckRedirectSafetyAsync(Arg.Any<UrlMapping>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(UrlSafetyCheckResult.Safe()));
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsPreview()
    {
        var shortCode = "preview123";
        var token = "valid-token";
        _tokenService.TryUnprotect(token, shortCode)
            .Returns(new AccessTokenPayload(shortCode, "Direct", DateTime.UtcNow.Ticks));

        var url = new UrlMapping
        {
            Id = 1,
            ShortCode = shortCode,
            LongUrl = "https://example.com/some/path",
            IsFlagged = false
        };
        _repo.GetByShortCodeAsync(shortCode).Returns(Task.FromResult<UrlMapping?>(url));

        var result = await _handler.Handle(new GetUrlPreviewQuery(shortCode, token), CancellationToken.None);

        result.Success.ShouldBeTrue();
        result.Status.ShouldBe(GetUrlPreviewStatus.Success);
        result.DestinationDomain.ShouldBe("example.com");
        result.DestinationUrl.ShouldBe("https://example.com/some/path");
    }

    [Fact]
    public async Task Handle_InvalidToken_ReturnsUnauthorized()
    {
        _tokenService.TryUnprotect(Arg.Any<string>(), Arg.Any<string>()).Returns((AccessTokenPayload?)null);

        var result = await _handler.Handle(new GetUrlPreviewQuery("code", "bad-token"), CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(GetUrlPreviewStatus.InvalidToken);
    }

    [Fact]
    public async Task Handle_DeletedOrNotFoundUrl_ReturnsNotFound()
    {
        _tokenService.TryUnprotect("token", "missing")
            .Returns(new AccessTokenPayload("missing", "Direct", DateTime.UtcNow.Ticks));
        _repo.GetByShortCodeAsync("missing").Returns(Task.FromResult<UrlMapping?>(null));

        var result = await _handler.Handle(new GetUrlPreviewQuery("missing", "token"), CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(GetUrlPreviewStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ExpiredUrl_ReturnsExpired()
    {
        _tokenService.TryUnprotect("token", "expired")
            .Returns(new AccessTokenPayload("expired", "Direct", DateTime.UtcNow.Ticks));
        var url = new UrlMapping
        {
            Id = 2,
            ShortCode = "expired",
            LongUrl = "https://example.com",
            ExpirationDate = DateTime.UtcNow.AddMinutes(-10)
        };
        _repo.GetByShortCodeAsync("expired").Returns(Task.FromResult<UrlMapping?>(url));

        var result = await _handler.Handle(new GetUrlPreviewQuery("expired", "token"), CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(GetUrlPreviewStatus.Expired);
    }

    [Fact]
    public async Task Handle_FlaggedUrl_ReturnsBlockedWithoutCreatingPreview()
    {
        _tokenService.TryUnprotect("token", "malicious")
            .Returns(new AccessTokenPayload("malicious", "Direct", DateTime.UtcNow.Ticks));
        var url = new UrlMapping
        {
            Id = 3,
            ShortCode = "malicious",
            LongUrl = "https://evil-phishing.com",
            IsFlagged = true,
            FlaggedReason = "URLhaus:malware"
        };
        _repo.GetByShortCodeAsync("malicious").Returns(Task.FromResult<UrlMapping?>(url));

        var result = await _handler.Handle(new GetUrlPreviewQuery("malicious", "token"), CancellationToken.None);

        // Per requirement: do NOT create or return destination preview page data for malicious URLs
        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(GetUrlPreviewStatus.Blocked);
        result.DestinationUrl.ShouldBeNull();
        result.DestinationDomain.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_UnsafeDetectedAtPreviewTime_ReturnsBlocked()
    {
        _tokenService.TryUnprotect("token", "compromised")
            .Returns(new AccessTokenPayload("compromised", "Direct", DateTime.UtcNow.Ticks));
        var url = new UrlMapping
        {
            Id = 4,
            ShortCode = "compromised",
            LongUrl = "https://compromised.com/bad",
            IsFlagged = false
        };
        _repo.GetByShortCodeAsync("compromised").Returns(Task.FromResult<UrlMapping?>(url));

        _safetyOrchestrator.CheckRedirectSafetyAsync(url, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(UrlSafetyCheckResult.Unsafe("scanner", "phishing", url.LongUrl, "Phishing site detected")));

        var result = await _handler.Handle(new GetUrlPreviewQuery("compromised", "token"), CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(GetUrlPreviewStatus.Blocked);
        result.DestinationUrl.ShouldBeNull();
    }
}
