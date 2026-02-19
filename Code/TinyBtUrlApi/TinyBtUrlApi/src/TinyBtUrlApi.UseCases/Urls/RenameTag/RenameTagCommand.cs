using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.RenameTag;

public record RenameTagCommand(int UrlId, string OldTag, string NewTag) : IRequest;
