using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.UpdateAlias;

public record UpdateAliasCommand(int Id, string NewAlias) : IRequest<string?>;
