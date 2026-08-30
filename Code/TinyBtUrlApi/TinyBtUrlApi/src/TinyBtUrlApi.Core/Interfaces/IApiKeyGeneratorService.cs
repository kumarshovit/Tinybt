using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IApiKeyGeneratorService
{
    ApiKeyGenerationResult GenerateKey();
    string HashKey(string rawKey);
}
