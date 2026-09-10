using System;

namespace TinyBtUrlApi.UseCases.ApiKeys.CreateApiKey;

public class CreateApiKeyResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public string RawKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
