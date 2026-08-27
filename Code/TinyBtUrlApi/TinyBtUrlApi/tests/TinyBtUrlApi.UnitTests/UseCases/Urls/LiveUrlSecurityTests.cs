using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure.Services;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class LiveUrlSecurityTests
{
    [Fact]
    public async Task ResolveChainAsync_RealCodeworksquickUrl_IsBlockedAndRejected()
    {
        // Arrange
        var options = Options.Create(new UrlSecurityOptions
        {
            MaxRedirectHops = 5,
            RedirectTimeoutMilliseconds = 5000,
            EnableRedirectChainValidation = true,
            EnableDnsValidation = true
        });

        var dnsService = new DnsLookupService(NullLogger<DnsLookupService>.Instance);
        var formatValidator = new UrlFormatValidator(options);
        var securityValidator = new UrlSecurityValidator(formatValidator, dnsService, options, NullLogger<UrlSecurityValidator>.Instance);

        var handler = new HttpClientHandler { AllowAutoRedirect = false };
        var httpClient = new HttpClient(handler);

        var resolver = new UrlRedirectResolver(httpClient, securityValidator, options, NullLogger<UrlRedirectResolver>.Instance);

        // Act
        var result = await resolver.ResolveChainAsync("https://www.codeworksquick.com/3TP5XSH/2BM8G1H2/", CancellationToken.None);

        // Assert: It must be blocked and fail with a proper message!
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("36vj3.speedyconnectedlink.com");
    }
}
