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

public class ContinueRedirectHandlerTests
{
    private readonly IUrlRepository _repo = Substitute.For<IUrlRepository>();
    private readonly IUrlAccessTokenService _tokenService = Substitute.For<IUrlAccessTokenService>();
    private readonly ContinueRedirectHandler _handler;

    public ContinueRedirectHandlerTests()
    {
        _handler = new ContinueRedirectHandler(_repo, _tokenService);
    }

    [Fact]
    public async Task Handle_ValidToken_IncrementsClickCountAndLogsClick()
    {
        var shortCode = "continue123";
        var token = "valid-token";
        _tokenService.TryUnprotect(token, shortCode)
            .Returns(new AccessTokenPayload(shortCode, "QRCode", DateTime.UtcNow.Ticks));

        var url = new UrlMapping
        {
            Id = 1,
            ShortCode = shortCode,
            LongUrl = "https://example.com/target",
            ClickCount = 5,
            IsFlagged = false
        };
        _repo.GetByShortCodeAsync(shortCode).Returns(Task.FromResult<UrlMapping?>(url));

        var command = new ContinueRedirectCommand(
            ShortCode: shortCode,
            Token: token,
            UserAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36",
            OS: "Windows",
            Country: "US",
            DeviceLanguage: "en-US",
            Referrer: "https://google.com",
            DeviceType: "Desktop",
            IpAddress: "203.0.113.1",
            RawHeaders: "User-Agent: ..."
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.ShouldBeTrue();
        result.Status.ShouldBe(ContinueRedirectStatus.Success);
        result.RedirectTo.ShouldBe("https://example.com/target");

        // 🎯 ClickCount must be incremented
        url.ClickCount.ShouldBe(6);

        // Click must be logged with payload source "QRCode"
        await _repo.Received(1).LogClickAsync(
            Arg.Is<ClickLog>(c =>
                c.ShortCode == shortCode &&
                c.Source == "QRCode" &&
                c.Browser == "Chrome" &&
                c.OS == "Windows"
            ),
            Arg.Any<CancellationToken>()
        );
        await _repo.Received(1).UpdateAsync(url);
    }

    [Fact]
    public async Task Handle_InvalidToken_DoesNotIncrementClickCount()
    {
        _tokenService.TryUnprotect(Arg.Any<string>(), Arg.Any<string>()).Returns((AccessTokenPayload?)null);

        var command = new ContinueRedirectCommand("code", "invalid-token", null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(ContinueRedirectStatus.InvalidToken);
        await _repo.DidNotReceive().LogClickAsync(Arg.Any<ClickLog>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_FlaggedUrl_ReturnsBlockedAndDoesNotIncrementClick()
    {
        var shortCode = "flagged";
        var token = "token";
        _tokenService.TryUnprotect(token, shortCode)
            .Returns(new AccessTokenPayload(shortCode, "Direct", DateTime.UtcNow.Ticks));

        var url = new UrlMapping
        {
            Id = 2,
            ShortCode = shortCode,
            LongUrl = "https://evil.com",
            IsFlagged = true,
            FlaggedReason = "ThreatIntelligence",
            ClickCount = 0
        };
        _repo.GetByShortCodeAsync(shortCode).Returns(Task.FromResult<UrlMapping?>(url));

        var command = new ContinueRedirectCommand(shortCode, token, null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(ContinueRedirectStatus.Blocked);
        url.ClickCount.ShouldBe(0);
        await _repo.DidNotReceive().LogClickAsync(Arg.Any<ClickLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExpiredUrl_ReturnsExpiredAndDoesNotIncrementClick()
    {
        var shortCode = "expired";
        var token = "token";
        _tokenService.TryUnprotect(token, shortCode)
            .Returns(new AccessTokenPayload(shortCode, "Direct", DateTime.UtcNow.Ticks));

        var url = new UrlMapping
        {
            Id = 3,
            ShortCode = shortCode,
            LongUrl = "https://example.com",
            ExpirationDate = DateTime.UtcNow.AddMinutes(-5),
            ClickCount = 2
        };
        _repo.GetByShortCodeAsync(shortCode).Returns(Task.FromResult<UrlMapping?>(url));

        var command = new ContinueRedirectCommand(shortCode, token, null, null, null, null, null, null, null, null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.Status.ShouldBe(ContinueRedirectStatus.Expired);
        url.ClickCount.ShouldBe(2);
        await _repo.DidNotReceive().LogClickAsync(Arg.Any<ClickLog>(), Arg.Any<CancellationToken>());
    }
}
