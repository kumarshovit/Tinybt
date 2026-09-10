using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.ApiKeys.DeleteApiKey;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.Web.Endpoints.ApiKeys.Requests;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys;

public class DeleteApiKeyEndpoint : Endpoint<DeleteApiKeyRequest>
{
    private readonly IMediator _mediator;

    public DeleteApiKeyEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/v1/api-keys/{Id}");
        Options(x => x.RequireRateLimiting(RateLimitConfigs.ApiKeyManagementPolicy));
        Description(x => x
            .WithTags("Developer API - Key Management")
            .WithSummary("Revoke an API key")
            .WithDescription("Revokes an API key owned by the authenticated user. Returns 404 for non-existent or unowned keys to prevent enumeration."));
    }

    public override async Task HandleAsync(DeleteApiKeyRequest req, CancellationToken ct)
    {
        var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var command = new DeleteApiKeyCommand(req.Id, userId);
        var success = await _mediator.Send(command, ct);

        // If revocation failed (not found, already revoked, or cross-ownership mismatch), 
        // return 404 to explicitly prevent resource enumeration.
        if (!success)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}
