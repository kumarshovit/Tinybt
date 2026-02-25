using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.UpdateAlias;

public record UpdateAliasCommand(int Id, string NewAlias) : IRequest<UrlDto?>;
