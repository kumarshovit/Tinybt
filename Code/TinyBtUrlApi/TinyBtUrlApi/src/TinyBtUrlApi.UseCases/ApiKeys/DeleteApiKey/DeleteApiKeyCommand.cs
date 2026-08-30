using Mediator;

namespace TinyBtUrlApi.UseCases.ApiKeys.DeleteApiKey;

public record DeleteApiKeyCommand(
    int Id,
    int UserId
) : IRequest<bool>;
