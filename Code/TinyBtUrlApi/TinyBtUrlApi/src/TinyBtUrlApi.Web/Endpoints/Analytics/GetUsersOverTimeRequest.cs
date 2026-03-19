public class GetUsersOverTimeRequest
{
  public DateTime start { get; set; }
  public DateTime end { get; set; }
  public string viewType { get; set; } = "daily";
}
