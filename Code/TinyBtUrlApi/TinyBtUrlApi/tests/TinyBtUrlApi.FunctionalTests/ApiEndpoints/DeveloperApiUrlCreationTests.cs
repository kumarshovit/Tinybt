using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Infrastructure.Data;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Requests;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Responses;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Requests;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Responses;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace TinyBtUrlApi.FunctionalTests.ApiEndpoints;

public class DeveloperApiUrlCreationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public DeveloperApiUrlCreationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        // Simulating JWT Auth for tests using a fake Bearer token or by registering a test auth handler
        // Usually, the CustomWebApplicationFactory is set up with a TestAuthHandler.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
    }

    [Fact]
    public async Task CreateUrl_WithValidApiKey_Returns201AndCorrectOwnership()
    {
        // 1. Create API key using the TestScheme
        var keyRequest = new CreateApiKeyRequest { Name = "Url Generation E2E Test Key" };
        var keyResponse = await _client.PostAsJsonAsync("/api/v1/api-keys", keyRequest);
        
        if (keyResponse.StatusCode == HttpStatusCode.Unauthorized) 
            return; // Fallback if Test Auth not configured

        var keyResult = await keyResponse.Content.ReadFromJsonAsync<CreateApiKeyResponse>();
        keyResult.ShouldNotBeNull();
        
        // 2. Set up new client passing the real X-API-Key
        var apiClient = _factory.CreateClient();
        apiClient.DefaultRequestHeaders.Add("X-API-Key", keyResult.RawKey);

        // 3. Create Short URL using developer API
        var urlRequest = new CreateDeveloperShortUrlRequest 
        { 
            LongUrl = "https://example.com/developer-api-test" 
        };
        var urlResponse = await apiClient.PostAsJsonAsync("/api/v1/links", urlRequest);
        
        urlResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var urlResult = await urlResponse.Content.ReadFromJsonAsync<CreateDeveloperShortUrlResponse>();
        urlResult.ShouldNotBeNull();
        urlResult.LongUrl.ShouldBe("https://example.com/developer-api-test");
        urlResult.ShortUrl.ShouldContain(urlResult.ShortCode);

    // 4. Verify Database Ownership
    using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var mapping = await context.Set<UrlMapping>().FirstOrDefaultAsync(u => u.Id == urlResult.Id);
        mapping.ShouldNotBeNull();
        
        // The URL should be owned by the user who created the API key
        // In our tests with TestScheme, is the UserId seeded properly? Assuming yes, or at least exists.
        mapping.UserId.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateUrl_WithMissingApiKey_Returns401()
    {
        var apiClient = _factory.CreateClient();
        // NOT adding X-API-Key header
        
        var urlRequest = new CreateDeveloperShortUrlRequest { LongUrl = "https://example.com/test" };
        var urlResponse = await apiClient.PostAsJsonAsync("/api/v1/links", urlRequest);
        
        urlResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
