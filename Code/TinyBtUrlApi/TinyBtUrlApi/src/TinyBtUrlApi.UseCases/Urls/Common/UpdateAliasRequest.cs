using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Common;

public class UpdateAliasRequest
{
  public string NewAlias { get; set; } = string.Empty;
}
