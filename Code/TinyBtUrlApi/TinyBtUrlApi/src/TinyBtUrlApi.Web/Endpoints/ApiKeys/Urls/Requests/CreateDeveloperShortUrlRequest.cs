using System;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Requests;

public class CreateDeveloperShortUrlRequest
{
    public string LongUrl { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Password { get; set; }
}
