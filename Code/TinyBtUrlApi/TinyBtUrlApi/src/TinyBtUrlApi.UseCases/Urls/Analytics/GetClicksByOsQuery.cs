using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

public record GetClicksByOsQuery()
    : IRequest<List<ClicksByOsDto>>;
