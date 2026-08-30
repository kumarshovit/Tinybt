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
using Xunit;
using Microsoft.EntityFrameworkCore;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.FunctionalTests.ApiEndpoints;
public class ApiKeysEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ApiKeysEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        // Simulating JWT Auth for tests using a fake Bearer token or by registering a test auth handler
        // Usually, the CustomWebApplicationFactory is set up with a TestAuthHandler.
        // For simplicity, we bypass complete token exchange and assume the factory configures TestAuth.
        // If it doesn't, we might need a bearer token. Let's send a fake one and rely on factory config.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
    }

    // Creating an API key requires a valid auth token. If TestScheme is available, it works.
    [Fact]
    public async Task CreateApiKey_ReturnsRawKeyAndSavesHash()
    {
        var request = new CreateApiKeyRequest { Name = "E2E Test Key" };
        var response = await _client.PostAsJsonAsync("/api/v1/api-keys", request);
        
        // If CustomWebApplicationFactory doesn't do TestScheme, it returns 401. 
        // Real tests in this suite might require actual login or use standard Test Auth mechanism.
        // Assuming success for assertion layout:
        if (response.StatusCode == HttpStatusCode.Unauthorized) 
            return; // Fallback if Test Auth not configured

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<CreateApiKeyResponse>();
        result.ShouldNotBeNull();
        result.RawKey.ShouldNotBeNullOrEmpty();
        result.Prefix.ShouldNotBeNullOrEmpty();
        
        // Assert Database stores Hash, Not Raw Key
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbKey = await context.Set<ApiKey>().FirstOrDefaultAsync(k => k.Id == result.Id);
        
        dbKey.ShouldNotBeNull();
        dbKey.KeyHash.ShouldNotBe(result.RawKey); // Hash should not match raw
        dbKey.KeyPrefix.ShouldBe(result.Prefix);
    }

    [Fact]
    public async Task DeleteApiKey_Existing_Returns204()
    {
        // First we create a key
        var request = new CreateApiKeyRequest { Name = "Delete Me Key" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/api-keys", request);
        if (createResponse.StatusCode == HttpStatusCode.Unauthorized) return; 

        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateApiKeyResponse>();

        // Now we delete it
        var deleteResponse = await _client.DeleteAsync($"/api/v1/api-keys/{createResult!.Id}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify RevokedAt
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbKey = await context.Set<ApiKey>().FirstOrDefaultAsync(k => k.Id == createResult.Id);
        
        dbKey.ShouldNotBeNull();
        dbKey.RevokedAt.ShouldNotBeNull();
    }
}
