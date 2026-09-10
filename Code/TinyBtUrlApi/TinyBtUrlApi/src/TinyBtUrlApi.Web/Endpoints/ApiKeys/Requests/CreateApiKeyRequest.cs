namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Requests;

public class CreateApiKeyRequest
{
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}
