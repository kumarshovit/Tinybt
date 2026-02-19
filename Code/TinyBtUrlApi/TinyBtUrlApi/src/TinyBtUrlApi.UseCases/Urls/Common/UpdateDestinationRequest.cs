using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Common;

public class UpdateDestinationRequest
{
  public string NewLongUrl { get; set; } = string.Empty;
}
