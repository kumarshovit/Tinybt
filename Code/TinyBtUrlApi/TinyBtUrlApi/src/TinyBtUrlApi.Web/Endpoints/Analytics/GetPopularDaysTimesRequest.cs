//namespace TinyBtUrlApi.Web.Endpoints.Analytics;

//public class GetPopularDaysTimesRequest
//{
//  public string ShortCode { get; set; } = default!;
//}

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetPopularDaysTimesRequest
{
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
}
