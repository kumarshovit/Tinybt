using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Infrastructure.Services;
using Xunit;

namespace TinyBtUrlApi.UnitTests.Infrastructure.Services;

public class AbstractEmailValidationServiceTests
{
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }

    private static AbstractEmailValidationService CreateService(
        Func<HttpRequestMessage, HttpResponseMessage> responseFactory,
        string apiKey = "test_key",
        int timeoutSeconds = 3)
    {
        var handler = new MockHttpMessageHandler(responseFactory);
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new AbstractEmailValidationOptions
        {
            ApiKey = apiKey,
            BaseUrl = "https://emailreputation.abstractapi.com/v1/",
            TimeoutSeconds = timeoutSeconds
        });

        return new AbstractEmailValidationService(httpClient, options, NullLogger<AbstractEmailValidationService>.Instance);
    }

    [Fact]
    public async Task ValidateEmailAsync_ValidEmail_ReturnsSuccess()
    {
        var json = """
        {
            "email": "user@legitdomain.com",
            "deliverability": "DELIVERABLE",
            "is_valid_format": { "value": true, "text": "TRUE" },
            "is_disposable_email": { "value": false, "text": "FALSE" },
            "is_mx_found": { "value": true, "text": "TRUE" },
            "is_smtp_valid": { "value": true, "text": "TRUE" }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("user@legitdomain.com");

        result.IsValid.ShouldBeTrue();
        result.IsDisposable.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateEmailAsync_ConfirmedDisposableEmail_ReturnsDisposableRejection()
    {
        var json = """
        {
            "email": "throwaway@mailinator.com",
            "deliverability": "DELIVERABLE",
            "is_valid_format": { "value": true, "text": "TRUE" },
            "is_disposable_email": { "value": true, "text": "TRUE" }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("throwaway@mailinator.com");

        result.IsValid.ShouldBeFalse();
        result.IsDisposable.ShouldBeTrue();
        result.RejectionReason.ShouldContain("Disposable or temporary email addresses are not permitted.");
    }

    [Fact]
    public async Task ValidateEmailAsync_FlatDisposableFlag_ReturnsDisposableRejection()
    {
        var json = """
        {
            "email": "throwaway@guerrillamail.com",
            "deliverability": "DELIVERABLE",
            "is_valid_format": true,
            "is_disposable": true
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("throwaway@guerrillamail.com");

        result.IsValid.ShouldBeFalse();
        result.IsDisposable.ShouldBeTrue();
        result.RejectionReason.ShouldContain("Disposable or temporary email addresses are not permitted.");
    }

    [Fact]
    public async Task ValidateEmailAsync_InvalidFormat_ReturnsFormatRejection()
    {
        var json = """
        {
            "email": "not-an-email",
            "deliverability": "UNDELIVERABLE",
            "is_valid_format": { "value": false, "text": "FALSE" }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("not-an-email@example.com");

        result.IsValid.ShouldBeFalse();
        result.RejectionReason.ShouldBe("Invalid email format.");
    }

    [Fact]
    public async Task ValidateEmailAsync_UndeliverableEmail_ReturnsUndeliverableRejection()
    {
        var json = """
        {
            "email": "nonexistent@example.com",
            "deliverability": "UNDELIVERABLE",
            "is_valid_format": { "value": true, "text": "TRUE" },
            "is_disposable_email": { "value": false, "text": "FALSE" }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("nonexistent@example.com");

        result.IsValid.ShouldBeFalse();
        result.RejectionReason.ShouldBe("Email address is undeliverable.");
    }

    [Fact]
    public async Task ValidateEmailAsync_OptionalFieldsNullOrMissing_DoesNotReject()
    {
        var json = """
        {
            "email": "user@customdomain.io",
            "deliverability": "DELIVERABLE",
            "is_valid_format": { "value": true, "text": "TRUE" },
            "is_disposable_email": { "value": false, "text": "FALSE" },
            "is_mx_found": null,
            "is_smtp_valid": null
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("user@customdomain.io");

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateEmailAsync_MissingMxFound_ReturnsMxRejection()
    {
        var json = """
        {
            "email": "user@nomxdomain.com",
            "deliverability": "DELIVERABLE",
            "is_valid_format": { "value": true, "text": "TRUE" },
            "is_disposable_email": { "value": false, "text": "FALSE" },
            "is_mx_found": { "value": false, "text": "FALSE" }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("user@nomxdomain.com");

        result.IsValid.ShouldBeFalse();
        result.RejectionReason.ShouldBe("Email domain cannot receive mail.");
    }

    [Fact]
    public async Task ValidateEmailAsync_Http429QuotaExceeded_ReturnsProviderUnavailable()
    {
        var service = CreateService(_ => new HttpResponseMessage((HttpStatusCode)429));

        var result = await service.ValidateEmailAsync("user@example.com");

        result.IsValid.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeFalse();
        result.RejectionReason.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");
    }

    [Fact]
    public async Task ValidateEmailAsync_Http500ServerError_ReturnsProviderUnavailable()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await service.ValidateEmailAsync("user@example.com");

        result.IsValid.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeFalse();
        result.RejectionReason.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");
    }

    [Fact]
    public async Task ValidateEmailAsync_Http401Unauthorized_ReturnsProviderUnavailable()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var result = await service.ValidateEmailAsync("user@example.com");

        result.IsValid.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeFalse();
        result.RejectionReason.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");
    }

    [Fact]
    public async Task ValidateEmailAsync_MissingApiKey_ReturnsProviderUnavailable()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK), apiKey: "");

        var result = await service.ValidateEmailAsync("user@example.com");

        result.IsValid.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeFalse();
        result.RejectionReason.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");
    }

    [Fact]
    public async Task ValidateEmailAsync_MalformedJson_ReturnsProviderUnavailable()
    {
        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("this is not json", Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("user@example.com");

        result.IsValid.ShouldBeFalse();
        result.IsProviderAvailable.ShouldBeFalse();
        result.RejectionReason.ShouldContain("We're temporarily unable to verify this email address. Please try again shortly.");
    }

    [Fact]
    public async Task ValidateEmailAsync_EmailReputationApi_ValidEmail_ReturnsSuccess()
    {
        var json = """
        {
            "email_address": "benjamin.richard@abstractapi.com",
            "suggested_correction": null,
            "email_deliverability": {
                "status": "deliverable",
                "status_detail": "valid_email",
                "is_format_valid": true,
                "is_smtp_valid": true,
                "is_mx_valid": true
            },
            "email_quality": {
                "score": 0.8,
                "is_free_email": false,
                "is_disposable": false
            }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("benjamin.richard@abstractapi.com");

        result.IsValid.ShouldBeTrue();
        result.IsDisposable.ShouldBeFalse();
    }

    [Fact]
    public async Task ValidateEmailAsync_EmailReputationApi_DisposableEmail_ReturnsDisposableRejection()
    {
        var json = """
        {
            "email_address": "fake@tempmail.com",
            "email_deliverability": {
                "status": "deliverable",
                "is_format_valid": true,
                "is_smtp_valid": true,
                "is_mx_valid": true
            },
            "email_quality": {
                "score": 0.1,
                "is_disposable": true
            }
        }
        """;

        var service = CreateService(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

        var result = await service.ValidateEmailAsync("fake@tempmail.com");

        result.IsValid.ShouldBeFalse();
        result.IsDisposable.ShouldBeTrue();
        result.RejectionReason.ShouldContain("Disposable or temporary email addresses are not permitted.");
    }
}
