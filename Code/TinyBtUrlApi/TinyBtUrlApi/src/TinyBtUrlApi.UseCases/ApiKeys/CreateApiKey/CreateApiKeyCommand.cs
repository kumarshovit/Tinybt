using Mediator;

namespace TinyBtUrlApi.UseCases.ApiKeys.CreateApiKey;

public record CreateApiKeyCommand(
    int UserId,
    string Name,
    DateTime? ExpiresAt = null
) : IRequest<CreateApiKeyResult>;
