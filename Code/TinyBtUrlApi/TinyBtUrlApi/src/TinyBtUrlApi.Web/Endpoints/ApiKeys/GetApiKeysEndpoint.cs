using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Responses;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys;

public class GetApiKeysEndpoint : EndpointWithoutRequest<GetApiKeysResponse>
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public GetApiKeysEndpoint(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    public override void Configure()
    {
        Get("/api/v1/api-keys");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Description(b => b
            .WithTags("API Keys")
            .Produces<GetApiKeysResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized));
        Summary(s =>
        {
            s.Summary = "Gets all Developer API keys for the authenticated user";
            s.Description = "Returns a safe metadata list of Developer API keys. The raw key and hash are never included in the response.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var keys = await _apiKeyRepository.GetByUserIdAsync(userId, ct);

        var response = new GetApiKeysResponse
        {
            ApiKeys = keys.Select(k => new ApiKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                KeyPrefix = k.KeyPrefix,
                CreatedAt = k.CreatedAt,
                ExpiresAt = k.ExpiresAt,
                LastUsedAt = k.LastUsedAt,
                RevokedAt = k.RevokedAt
            }).ToList()
        };

        await Send.OkAsync(response, ct);
    }
}
