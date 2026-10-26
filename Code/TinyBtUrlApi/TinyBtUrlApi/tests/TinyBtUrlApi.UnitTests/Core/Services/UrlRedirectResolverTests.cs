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
    private readonly IUrlSecurityValidator _securityValidator;
    private readonly IOptions<UrlSecurityOptions> _options;

    public UrlRedirectResolverTests()
    {
        _securityValidator = Substitute.For<IUrlSecurityValidator>();
        _securityValidator.ValidateUrlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(callInfo =>
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
            MaxAllowedRedirects = 2,
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
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://example.com/page");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://example.com/page");
        result.RedirectChain.ShouldBe(new[] { "https://example.com/page" });
    }

    [Fact]
    public async Task ResolveChainAsync_SingleRedirect_ReturnsOriginalAndFinalUrl()
    {
        // Arrange: 1 redirect (A -> B)
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://start.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.MovedPermanently);
                resp.Headers.Location = new Uri("https://destination.com/final");
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://start.com/");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://destination.com/final");
        result.RedirectChain.Count.ShouldBe(2);
        result.RedirectChain[0].ShouldBe("https://start.com/");
        result.RedirectChain[1].ShouldBe("https://destination.com/final");
    }

    [Fact]
    public async Task ResolveChainAsync_TwoRedirects_AllowedAndReturnsAllHops()
    {
        // Arrange: Exactly 2 redirects (A -> B -> C)
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
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

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
    public async Task ResolveChainAsync_ThreeRedirects_RejectedBeforeRequestingThirdRedirect()
    {
        // Arrange: 3 redirects (hop0 -> hop1 -> hop2 -> hop3)
        // Requests made should be hop0, hop1, hop2 (which attempts redirect to hop3 and is blocked)
        var requestedUrls = new List<string>();
        var handler = new MockHttpMessageHandler(req =>
        {
            var uri = req.RequestUri?.ToString() ?? string.Empty;
            requestedUrls.Add(uri);

            if (uri == "https://hop0.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://hop1.com/");
                return resp;
            }
            if (uri == "https://hop1.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://hop2.com/");
                return resp;
            }
            if (uri == "https://hop2.com/")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("https://hop3.com/");
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://hop0.com/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("maximum 2 redirects allowed");
        result.ErrorMessage.ShouldContain("too many redirects");

        // Ensure hop3 was never requested
        requestedUrls.ShouldNotContain("https://hop3.com/");
        requestedUrls.Count.ShouldBe(3); // Only hop0, hop1, hop2 requested
    }

    [Fact]
    public async Task ResolveChainAsync_RelativeLocationHeader_ResolvesCorrectly()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://site.com/start")
            {
                var resp = new HttpResponseMessage(HttpStatusCode.Found);
                resp.Headers.Location = new Uri("/relative-destination", UriKind.Relative);
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://site.com/start");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://site.com/relative-destination");
        result.RedirectChain.Count.ShouldBe(2);
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
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://public-start.com/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("Localhost and loopback addresses are not allowed");
    }

    [Fact]
    public async Task ResolveChainAsync_RedirectHopFlaggedByDnsThreat_FailsValidation()
    {
        // Arrange: Redirect hop to a known spam / sinkholed domain
        _securityValidator.ValidateUrlAsync(Arg.Is<string>(u => u.Contains("speedyconnectedlink")), Arg.Any<CancellationToken>())
            .Returns(UrlSecurityResult.CreateFailure("The destination domain '36vj3.speedyconnectedlink.com' is flagged as malicious/spam by DNS threat intelligence."));

        var handler = new MockHttpMessageHandler(req =>
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Found);
            resp.Headers.Location = new Uri("https://36vj3.speedyconnectedlink.com/?kw=1778");
            return resp;
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://www.codeworksquick.com/3TP5XSH/2BM8G1H2/");

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("flagged as malicious/spam");
    }

    [Fact]
    public async Task ResolveChainAsync_JavaScriptLocationReplace_FollowsRedirect()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://cloaked-landing.com/")
            {
                var html = @"<!DOCTYPE html><html><head><script>location.replace('https://real-destination.com/login');</script></head><body>Loading...</body></html>";
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html")
                };
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://cloaked-landing.com/");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://real-destination.com/login");
        result.RedirectChain.Count.ShouldBe(2);
        result.RedirectChain[0].ShouldBe("https://cloaked-landing.com/");
        result.RedirectChain[1].ShouldBe("https://real-destination.com/login");
    }

    [Fact]
    public async Task ResolveChainAsync_JavaScriptWindowLocationAssign_FollowsRedirect()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://js-redirect.com/")
            {
                var html = @"<html><head><script type=""text/javascript"">window.location.href = ""https://final-app.com/home"";</script></head></html>";
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html")
                };
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://js-redirect.com/");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://final-app.com/home");
        result.RedirectChain.Count.ShouldBe(2);
        result.RedirectChain[1].ShouldBe("https://final-app.com/home");
    }

    [Fact]
    public async Task ResolveChainAsync_HtmlMetaRefresh_FollowsRedirect()
    {
        // Arrange
        var handler = new MockHttpMessageHandler(req =>
        {
            if (req.RequestUri?.ToString() == "https://meta-refresh.com/")
            {
                var html = @"<html><head><meta http-equiv=""refresh"" content=""0; url=https://meta-destination.com/index""></head></html>";
                var resp = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html")
                };
                return resp;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var client = new HttpClient(handler);
        var resolver = new UrlRedirectResolver(client, _securityValidator, _options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://meta-refresh.com/");

        // Assert
        result.Success.ShouldBeTrue();
        result.FinalUrl.ShouldBe("https://meta-destination.com/index");
        result.RedirectChain.Count.ShouldBe(2);
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
