using Microsoft.AspNetCore.Authentication;

namespace TinyBtUrlApi.Web.Auth.ApiKey;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public string HeaderName { get; set; } = "X-API-Key";
}
