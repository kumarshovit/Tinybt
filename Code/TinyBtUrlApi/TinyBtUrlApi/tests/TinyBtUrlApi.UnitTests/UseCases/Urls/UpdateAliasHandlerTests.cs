using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.UseCases.Urls.UpdateAlias;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class UpdateAliasHandlerTests
{
    private readonly IUrlRepository _repo = Substitute.For<IUrlRepository>();
    private readonly UpdateAliasHandler _handler;

    public UpdateAliasHandlerTests()
    {
        _handler = new UpdateAliasHandler(_repo);
    }

    [Theory]
    [InlineData("about")]
    [InlineData("tools")]
    [InlineData("use-cases")]
    [InlineData("docs")]
    [InlineData("compare")]
    [InlineData("countries")]
    [InlineData("glossary")]
    [InlineData("robots.txt")]
    [InlineData("About")]
    [InlineData("TOOLS")]
    [InlineData("not-found")]
    public async Task Handle_ReservedAlias_ReturnsNull(string reservedSlug)
    {
        var command = new UpdateAliasCommand(1, reservedSlug, 10, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBeNull();
        await _repo.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<UrlMapping>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid alias with space")]
    [InlineData("bad_alias!")]
    [InlineData("slash/in/alias")]
    public async Task Handle_InvalidFormatAlias_ReturnsNull(string invalidAlias)
    {
        var command = new UpdateAliasCommand(1, invalidAlias, 10, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBeNull();
        await _repo.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_ExistingAlias_ReturnsNull()
    {
        _repo.ShortCodeExists("my-taken-alias").Returns(true);
        var command = new UpdateAliasCommand(1, "my-taken-alias", 10, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBeNull();
        await _repo.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<UrlMapping>());
    }

    [Fact]
    public async Task Handle_ValidAlias_NormalizesToLowerAndSucceeds()
    {
        _repo.ShortCodeExists("new-promo-link").Returns(false);
        var existingUrl = new UrlMapping
        {
            Id = 1,
            LongUrl = "https://example.com/target",
            ShortCode = "old-code",
            UserId = 10,
            ClickCount = 42
        };
        _repo.GetByIdAsync(1).Returns(existingUrl);

        var command = new UpdateAliasCommand(1, "  New-Promo-Link  ", 10, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.ShortCode.ShouldBe("new-promo-link");
        existingUrl.ShortCode.ShouldBe("new-promo-link");
        await _repo.Received(1).UpdateAsync(existingUrl);
    }

    [Fact]
    public async Task Handle_NonOwnerNonAdmin_ReturnsNull()
    {
        _repo.ShortCodeExists("valid-alias").Returns(false);
        var existingUrl = new UrlMapping
        {
            Id = 1,
            LongUrl = "https://example.com/target",
            ShortCode = "old-code",
            UserId = 10,
            ClickCount = 42
        };
        _repo.GetByIdAsync(1).Returns(existingUrl);

        // Different user ID, not admin
        var command = new UpdateAliasCommand(1, "valid-alias", 999, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBeNull();
        await _repo.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<UrlMapping>());
    }
}

