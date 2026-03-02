namespace TinyBtUrlApi.Web.Endpoints.Urls.Requests;

public class AddTagsRequest
{
  public List<string> Tags { get; set; } = new();
}
