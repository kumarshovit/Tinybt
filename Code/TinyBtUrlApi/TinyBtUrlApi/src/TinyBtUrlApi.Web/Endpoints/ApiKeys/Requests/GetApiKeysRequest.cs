using FastEndpoints;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Requests;

public class GetApiKeysRequest
{
    [QueryParam]
    public bool IncludeRevoked { get; set; } = false;
}
