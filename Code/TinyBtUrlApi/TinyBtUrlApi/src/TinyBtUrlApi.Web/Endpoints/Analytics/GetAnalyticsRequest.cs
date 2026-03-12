namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetAnalyticsRequest
{
  public DateTime From { get; set; }

  public DateTime To { get; set; }

  public string Type { get; set; } = "";

  public string? Link { get; set; }

  public string? Tag { get; set; }

}
