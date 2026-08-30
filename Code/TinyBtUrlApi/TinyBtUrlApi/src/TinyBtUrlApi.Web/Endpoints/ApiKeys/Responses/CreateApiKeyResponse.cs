using System;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Responses;

public class CreateApiKeyResponse
{
    public int Id { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public string RawKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
