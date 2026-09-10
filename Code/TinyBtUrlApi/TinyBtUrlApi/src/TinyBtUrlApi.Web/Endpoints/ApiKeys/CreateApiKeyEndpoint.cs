using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.ApiKeys.CreateApiKey;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Requests;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Responses;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys;

public class CreateApiKeyEndpoint : Endpoint<CreateApiKeyRequest, CreateApiKeyResponse>
{
    private readonly IMediator _mediator;

    public CreateApiKeyEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/v1/api-keys");
        Options(x => x.RequireRateLimiting(RateLimitConfigs.ApiKeyManagementPolicy));
        Description(x => x
            .WithTags("Developer API - Key Management")
            .WithSummary("Create a new API key")
            .WithDescription("Generates a new API key for the authenticated user. The raw key is returned once and never stored."));
        // By default, FastEndpoints demands authentication based on the globally registered schemes
        // which defaults to ASP.NET's JwtBearer scheme. We intentionally do not allow anonymous.
    }

    public override async Task HandleAsync(CreateApiKeyRequest req, CancellationToken ct)
    {
        var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var command = new CreateApiKeyCommand(userId, req.Name, req.ExpiresAt);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            AddError(result.Message);
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var response = new CreateApiKeyResponse
        {
            Id = result.Id,
            Prefix = result.Prefix,
            RawKey = result.RawKey,
            CreatedAt = result.CreatedAt,
            ExpiresAt = result.ExpiresAt
        };

        await Send.CreatedAtAsync<CreateApiKeyEndpoint>(new { id = result.Id }, response, cancellation: ct);
    }
}
