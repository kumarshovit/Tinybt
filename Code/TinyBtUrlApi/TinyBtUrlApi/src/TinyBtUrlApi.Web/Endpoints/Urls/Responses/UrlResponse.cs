namespace TinyBtUrlApi.Web.Endpoints.Urls.Responses;

public class UrlResponse
{
  public int Id { get; set; }
  public string? LongUrl { get; set; }
  public string? ShortCode { get; set; }
  public string? ShortUrl { get; set; }
  public DateTime? ExpirationDate { get; set; }
  public int ClickCount { get; set; }
  public List<string>? Tags { get; set; }
}
