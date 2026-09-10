using System.Threading;
using System.Threading.Tasks;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.ApiKeys.CreateApiKey;

public class CreateApiKeyHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResult>
{
    private const int MaxApiKeysPerUser = 5;

    private readonly IApiKeyGeneratorService _apiKeyGeneratorService;
    private readonly IApiKeyRepository _apiKeyRepository;

    public CreateApiKeyHandler(IApiKeyGeneratorService apiKeyGeneratorService, IApiKeyRepository apiKeyRepository)
    {
        _apiKeyGeneratorService = apiKeyGeneratorService;
        _apiKeyRepository = apiKeyRepository;
    }

    public async ValueTask<CreateApiKeyResult> Handle(CreateApiKeyCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new CreateApiKeyResult { Success = false, Message = "API Key Name is required." };
        }

        var activeKeyCount = await _apiKeyRepository.CountActiveByUserIdAsync(request.UserId, ct);
        if (activeKeyCount >= MaxApiKeysPerUser)
        {
            return new CreateApiKeyResult
            {
                Success = false,
                Message = $"Maximum number of active API keys ({MaxApiKeysPerUser}) reached. Revoke an existing key before creating a new one."
            };
        }

        var generatedKeyInfo = _apiKeyGeneratorService.GenerateKey();

        var apiKeyEntity = await _apiKeyRepository.CreateAsync(
            request.UserId,
            request.Name,
            generatedKeyInfo,
            request.ExpiresAt,
            ct);

        return new CreateApiKeyResult
        {
            Success = true,
            Id = apiKeyEntity.Id,
            Prefix = generatedKeyInfo.Prefix,
            RawKey = generatedKeyInfo.RawKey,
            CreatedAt = apiKeyEntity.CreatedAt,
            ExpiresAt = apiKeyEntity.ExpiresAt
        };
    }
}
