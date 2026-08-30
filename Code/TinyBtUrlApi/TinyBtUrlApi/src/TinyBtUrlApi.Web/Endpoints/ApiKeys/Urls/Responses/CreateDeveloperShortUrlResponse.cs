using System;

namespace TinyBtUrlApi.Web.Endpoints.ApiKeys.Urls.Responses;

public class CreateDeveloperShortUrlResponse
{
    public int Id { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
    public string LongUrl { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPasswordProtected { get; set; }
}
