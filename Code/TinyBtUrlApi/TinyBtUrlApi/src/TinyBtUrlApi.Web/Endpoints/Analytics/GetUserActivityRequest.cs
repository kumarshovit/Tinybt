
using Microsoft.AspNetCore.Mvc;

namespace TinyBtUrlApi.Web.Endpoints.Analytics;

public class GetUserActivityRequest
{
  [FromRoute]
  public int UserId { get; set; }
}
