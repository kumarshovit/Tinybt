using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Responses;

public class GetApiKeysResponse
{
    [JsonPropertyName("apiKeys")]
    public IEnumerable<ApiKeyDto> ApiKeys { get; set; } = new List<ApiKeyDto>();
}

public class ApiKeyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public System.DateTime CreatedAt { get; set; }
    public System.DateTime? ExpiresAt { get; set; }
    public System.DateTime? LastUsedAt { get; set; }
    public System.DateTime? RevokedAt { get; set; }
}
