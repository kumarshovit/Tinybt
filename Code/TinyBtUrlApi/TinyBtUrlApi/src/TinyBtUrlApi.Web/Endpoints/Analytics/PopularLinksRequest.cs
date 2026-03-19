namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class PopularLinksRequest
{
  public DateTime From { get; set; }

  public DateTime To { get; set; }
  public string? Link { get; set; }

  public string? Tag { get; set; }

  public int? UserId { get; set; }
}
