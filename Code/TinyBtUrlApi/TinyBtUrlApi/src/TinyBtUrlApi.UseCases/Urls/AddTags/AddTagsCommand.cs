using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.AddTags;

public record AddTagsCommand(int UrlId, List<string> Tags) : IRequest;
