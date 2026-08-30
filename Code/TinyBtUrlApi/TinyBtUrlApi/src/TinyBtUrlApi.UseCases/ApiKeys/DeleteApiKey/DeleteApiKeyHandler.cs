using System.Threading;
using System.Threading.Tasks;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.ApiKeys.DeleteApiKey;

public class DeleteApiKeyHandler : IRequestHandler<DeleteApiKeyCommand, bool>
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public DeleteApiKeyHandler(IApiKeyRepository apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    public async ValueTask<bool> Handle(DeleteApiKeyCommand request, CancellationToken ct)
    {
        return await _apiKeyRepository.RevokeAsync(request.Id, request.UserId, ct);
    }
}
