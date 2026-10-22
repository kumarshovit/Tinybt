using Microsoft.Extensions.Options;
using Shouldly;
using TinyBtUrlApi.Core.Configuration;
using TinyBtUrlApi.Core.Services;
using Xunit;

namespace TinyBtUrlApi.UnitTests.UseCases.Urls;

public class UrlFormatValidatorTests
{
    private readonly UrlFormatValidator _validator;

    public UrlFormatValidatorTests()
    {
        var options = Options.Create(new UrlSecurityOptions
        {
            MaxUrlLength = 2048,
            AllowHttp = false,
            EnableDnsValidation = true
        });

        _validator = new UrlFormatValidator(options);
    }

    [Fact]
    public void ValidateFormat_TheReportedGoogleStoragePhishingUrl_IsBlocked()
    {
        // Arrange
        var attackUrl = "https://storage.googleapis.com/sdg54sd6fgh546rde5gt4ser6g4dfgtred/sdfgtedrf.html#cl/0_smd/85/3607692/3864/0/6ac3970187f304a6e5605c2a";

        // Act
        var result = _validator.ValidateFormat(attackUrl);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("cloud storage");
    }

    [Theory]
    [InlineData("https://mybucket.blob.core.windows.net/portal/login.html")]
    [InlineData("https://mybucket.s3.amazonaws.com/invoice.htm")]
    [InlineData("https://s3.amazonaws.com/mybucket/page.php")]
    [InlineData("https://app.firebaseapp.com/auth.js")]
    [InlineData("https://myworker.workers.dev/payload.svg")]
    public void ValidateFormat_CloudStorageRawHtmlAndScripts_AreBlocked(string cloudUrl)
    {
        var result = _validator.ValidateFormat(cloudUrl);

        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("cloud storage");
    }

    [Theory]
    [InlineData("https://bit.ly/malicious-link")]
    [InlineData("https://tinyurl.com/xyz123")]
    [InlineData("https://cutt.ly/phishing")]
    [InlineData("https://link.bt/recursive")]
    [InlineData("https://api.link.bt/chain")]
    public void ValidateFormat_ChainedShorteners_AreBlocked(string shortenerUrl)
    {
        var result = _validator.ValidateFormat(shortenerUrl);

        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("another URL shortener");
    }

    [Theory]
    [InlineData("https://192.168.1.1/admin")]
    [InlineData("https://45.33.32.156/path")]
    [InlineData("https://169.254.169.254/latest/meta-data")]
    [InlineData("https://5.230.173.220/")]
    public void ValidateFormat_RawIpAddressesAndServerIps_AreBlocked(string ipUrl)
    {
        var result = _validator.ValidateFormat(ipUrl);

        result.Success.ShouldBeFalse();
    }

    [Theory]
    [InlineData("https://xn--pypal-4ve.com/login")]
    [InlineData("https://secure.xn--google-sample.com")]
    public void ValidateFormat_PunycodeHomographDomains_AreBlocked(string punycodeUrl)
    {
        var result = _validator.ValidateFormat(punycodeUrl);

        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("Punycode");
    }

    [Theory]
    [InlineData("https://example.com/downloads/setup.exe")]
    [InlineData("https://example.com/files/payload.scr")]
    [InlineData("https://example.com/installer.iso")]
    [InlineData("https://example.com/script.bat")]
    public void ValidateFormat_DirectExecutableDownloads_AreBlocked(string exeUrl)
    {
        var result = _validator.ValidateFormat(exeUrl);

        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("executable");
    }

    [Theory]
    [InlineData("https://www.google.com/search?q=aspnet")]
    [InlineData("https://github.com/dotnet/aspnetcore")]
    [InlineData("https://en.wikipedia.org/wiki/URL_shortening")]
    [InlineData("https://docs.microsoft.com/en-us/dotnet/")]
    public void ValidateFormat_LegitimateUrls_PassSuccessfully(string legitimateUrl)
    {
        var result = _validator.ValidateFormat(legitimateUrl);

        result.Success.ShouldBeTrue();
        result.NormalizedUrl.ShouldNotBeNull();
    }
}
