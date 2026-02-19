using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.UpdateTagss;

public class UpdareTagsCommand
{
  public record UpdateTagsCommand(int UrlId, List<string> Tags) : IRequest;

}
