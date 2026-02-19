using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.RemoveTag;

public record RemoveTagCommand(int UrlId, string Tag) : IRequest;

