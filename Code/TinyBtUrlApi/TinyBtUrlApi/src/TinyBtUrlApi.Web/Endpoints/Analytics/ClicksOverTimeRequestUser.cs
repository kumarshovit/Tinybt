namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class ClicksOverTimeRequestUser
{
  public DateTime From { get; set; }

  public DateTime To { get; set; }
  public string? Link { get; set; }

  public string? Tag { get; set; }
}
