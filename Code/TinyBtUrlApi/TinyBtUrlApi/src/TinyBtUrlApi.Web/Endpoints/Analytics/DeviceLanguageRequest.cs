namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class DeviceLanguageRequest
{
  public DateTime From { get; set; }

  public DateTime To { get; set; }

  public string? Link { get; set; }

  public string? Tag { get; set; }
}
