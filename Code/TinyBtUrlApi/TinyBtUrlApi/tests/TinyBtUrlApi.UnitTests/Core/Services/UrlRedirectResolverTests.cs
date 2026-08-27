using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Infrastructure.Services;
using Xunit;

namespace TinyBtUrlApi.UnitTests.Core.Services;

public class UrlRedirectResolverTests
{
    private readonly IUrlFormatValidator _formatValidator;
    private readonly IOptions<UrlSecurityOptions> _options;

    public UrlRedirectResolverTests()
    {
        _formatValidator = Substitute.For<IUrlFormatValidator>();
        // Default format validator allows HTTPS
        _formatValidator.ValidateFormat(Arg.Any<string>()).Returns(callInfo =>
        {
            var url = callInfo.Arg<string>();
            if (url.Contains("127.0.0.1") || url.Contains("localhost"))
            {
                return UrlSecurityResult.CreateFailure("Localhost and loopback addresses are not allowed.");
            }
            return UrlSecurityResult.CreateSuccess(url);
        });

        _options = Options.Create(new UrlSecurityOptions
        {
            MaxRedirectHops = 3,
            RedirectTimeoutMilliseconds = 2000,
            EnableRedirectChainValidation = true
        });
    }

    [Fact]
    public async Task ResolveChainAsync_SingleUrlNoRedirect_ReturnsOriginalUrlInChain()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req => new HttpResponseMessage(HttpStatusCode.OK));
        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _formatValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://example.com/page");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://example.com/page");
        result.RedirectChain.ShouldBe(new[] { "https://example.com/page" });
    }

    [Fact]
    public async Task ResolveChainAsync_MultiHopRedirect_ReturnsAllHopsAndFinalUrl()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://start.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.MovedPermanently);
                resp.Headers.Location = new Uri("https://middle.com/step2");
                return resp;
            }
            if (req.RequestUri?.ToString() == "https://middle.com/step2")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://destination.com/final");
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _formatValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://start.com/");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://destination.com/final");
        result.RedirectChain.Count.ShouldBe(3);
        result.RedirectChain[0].ShouldBe("https://start.com/");
        result.RedirectChain[1].ShouldBe("https://middle.com/step2");
        result.RedirectChain[2].ShouldBe("https://destination.com/final");
    }

    [Fact]
    public async Task ResolveChainAsync_CircularLoop_FailsWithLoopError()
    {
        // Arrange: A -> B -> A
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://loop-a.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://loop-b.com/");
                return resp;
            }
            if (req.RequestUri?.ToString() == "https://loop-b.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://loop-a.com/");
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _formatValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://loop-a.com/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldContain("Circular redirect loop detected");
    }

    [Fact]
    public async Task ResolveChainAsync_ExceedsMaxHops_FailsWithMaxHopsError()
    {
        // Arrange: Chain of 5 hops with MaxRedirectHops = 3
        int counter = 0;
        var handler = new MockHttpMessageHandler(req =>
        {
            counter++;
            var resp = new HttpResponseMessage(HttpStatusCode.Found);
            resp.Headers.Location = new Uri($"https://hop{counter}.com/");
            return resp;
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _formatValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://hop0.com/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldContain("exceeded the maximum allowed redirect limit");
    }

    [Fact]
    public async Task ResolveChainAsync_RedirectToLocalhostSSRF_FailsValidation()
    {
        // Arrange: Public link redirects to internal loopback
        var handler = new MockHttpMessageHandler(req =>
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Found);
            resp.Headers.Location = new Uri("https://127.0.0.1/admin");
            return resp;
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _formatValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://public-start.com/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldContain("Localhost and loopback addresses are not allowed");
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
