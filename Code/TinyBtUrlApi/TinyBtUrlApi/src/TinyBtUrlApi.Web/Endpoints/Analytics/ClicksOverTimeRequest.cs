namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class ClicksOverTimeRequest
{
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public string ViewType { get; set; } = "daily";
}
