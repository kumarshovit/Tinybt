namespace TinyBtUrlApi.Core.Models;

public class ApiKeyGenerationResult
{
    public string RawKey { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}
