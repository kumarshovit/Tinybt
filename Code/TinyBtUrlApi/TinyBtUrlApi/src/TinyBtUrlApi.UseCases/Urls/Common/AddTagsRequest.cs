using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Common;

public class AddTagsRequest
{
  public List<string> Tags { get; set; } = new();
}
