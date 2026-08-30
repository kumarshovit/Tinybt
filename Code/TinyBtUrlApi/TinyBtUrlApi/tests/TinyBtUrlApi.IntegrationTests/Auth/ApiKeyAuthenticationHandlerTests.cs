using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Web.Auth.ApiKey;
using Xunit;
using System.Security.Claims;

namespace TinyBtUrlApi.IntegrationTests.Auth;

public class ApiKeyAuthenticationHandlerTests
{
    private readonly IApiKeyRepository _repoMock;
    private readonly IApiKeyGeneratorService _genMock;
    private readonly IOptionsMonitor<ApiKeyAuthenticationOptions> _optionsMock;
    private readonly ApiKeyAuthenticationHandler _handler;

    public ApiKeyAuthenticationHandlerTests()
    {
        _repoMock = Substitute.For<IApiKeyRepository>();
        _genMock = Substitute.For<IApiKeyGeneratorService>();

        _optionsMock = Substitute.For<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        _optionsMock.Get(Arg.Any<string>()).Returns(new ApiKeyAuthenticationOptions { HeaderName = "X-API-Key" });

        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        var encoder = Substitute.For<UrlEncoder>();
        var timeProvider = TimeProvider.System;

        _handler = new ApiKeyAuthenticationHandler(_optionsMock, loggerFactory, encoder, timeProvider, _repoMock, _genMock);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_MissingKey_ReturnsNoResult()
    {
        var context = new DefaultHttpContext();
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        var result = await _handler.AuthenticateAsync(); // Calls internal HandleAuthenticateAsync

        result.None.ShouldBeTrue(); // Translates to 401 later in framework logic if challenged.
    }

    [Fact]
    public async Task HandleAuthenticateAsync_MalformedKey_ReturnsFail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = "invalid_prefix_123456";
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        var result = await _handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
        result.Failure.ShouldNotBeNull();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_InvalidKey_ReturnsFail()
    {
        var rawKey = "lbt_live_nonexistent";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = rawKey;
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        _genMock.HashKey(rawKey).Returns("fake_hash");
        _repoMock.FindByHashAsync("fake_hash", Arg.Any<CancellationToken>()).Returns((ApiKey?)null);

        var result = await _handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_RevokedKey_ReturnsFail()
    {
        var rawKey = "lbt_live_revoked";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = rawKey;
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        _genMock.HashKey(rawKey).Returns("revoked_hash");
        _repoMock.FindByHashAsync("revoked_hash", Arg.Any<CancellationToken>()).Returns(new ApiKey { RevokedAt = DateTime.UtcNow });

        var result = await _handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ExpiredKey_ReturnsFail()
    {
        var rawKey = "lbt_live_expired";
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = rawKey;
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        _genMock.HashKey(rawKey).Returns("expired_hash");
        _repoMock.FindByHashAsync("expired_hash", Arg.Any<CancellationToken>()).Returns(new ApiKey { ExpiresAt = DateTime.UtcNow.AddSeconds(-1) });

        var result = await _handler.AuthenticateAsync();

        result.Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ValidKey_ReturnsSuccessWithUserId()
    {
        var rawKey = "lbt_live_valid";
        var expectedUserId = 99;
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = rawKey;
        await _handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), context);

        _genMock.HashKey(rawKey).Returns("valid_hash");
        _repoMock.FindByHashAsync("valid_hash", Arg.Any<CancellationToken>()).Returns(new ApiKey { UserId = expectedUserId, Name = "Test Key" });

        var result = await _handler.AuthenticateAsync();

        result.Succeeded.ShouldBeTrue();
        result.Principal.ShouldNotBeNull();
        result.Principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Value == expectedUserId.ToString()).ShouldBeTrue();
    }
}
