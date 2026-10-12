using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.SharedKernel;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.UseCases.Account.VerifyEmail;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Account.VerifyEmail;

public class VerifyEmailHandlerTests
{
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly VerifyEmailHandler _handler;

    public VerifyEmailHandlerTests()
    {
        _handler = new VerifyEmailHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidTokenWithin24Hours_MarksUserVerifiedAndClearsToken()
    {
        // Arrange: token expiring in future (+12 hours)
        var expiryUnix = DateTimeOffset.UtcNow.AddHours(12).ToUnixTimeSeconds();
        var token = $"{Guid.NewGuid():N}.{expiryUnix}";

        var user = new User
        {
            Email = "user@example.com",
            IsEmailVerified = false,
            EmailVerificationToken = token
        };

        _repository.FirstOrDefaultAsync(Arg.Any<UserByVerificationTokenSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _handler.Handle(new VerifyEmailQuery(token));

        // Assert
        result.ShouldContain("Email verified successfully");
        user.IsEmailVerified.ShouldBeTrue();
        user.EmailVerificationToken.ShouldBeNull();

        await _repository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExpiredTokenOlderThan24Hours_ReturnsExpiredMessageAndDoesNotVerify()
    {
        // Arrange: token expired 2 hours ago
        var expiryUnix = DateTimeOffset.UtcNow.AddHours(-2).ToUnixTimeSeconds();
        var token = $"{Guid.NewGuid():N}.{expiryUnix}";

        var user = new User
        {
            Email = "user@example.com",
            IsEmailVerified = false,
            EmailVerificationToken = token
        };

        _repository.FirstOrDefaultAsync(Arg.Any<UserByVerificationTokenSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _handler.Handle(new VerifyEmailQuery(token));

        // Assert
        result.ShouldContain("Verification link has expired");
        user.IsEmailVerified.ShouldBeFalse();
        user.EmailVerificationToken.ShouldBe(token);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InvalidTokenNotFound_ReturnsInvalidOrExpired()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<UserByVerificationTokenSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _handler.Handle(new VerifyEmailQuery("nonexistent-token"));

        // Assert
        result.ShouldBe("Invalid or expired token.");
    }

    [Fact]
    public async Task Handle_LegacyTokenWithoutTimestamp_VerifiesSuccessfullyForBackwardCompatibility()
    {
        // Arrange: legacy plain GUID token without dot or timestamp
        var token = Guid.NewGuid().ToString();

        var user = new User
        {
            Email = "legacy@example.com",
            IsEmailVerified = false,
            EmailVerificationToken = token
        };

        _repository.FirstOrDefaultAsync(Arg.Any<UserByVerificationTokenSpec>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await _handler.Handle(new VerifyEmailQuery(token));

        // Assert
        result.ShouldContain("Email verified successfully");
        user.IsEmailVerified.ShouldBeTrue();
        user.EmailVerificationToken.ShouldBeNull();

        await _repository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }
}
