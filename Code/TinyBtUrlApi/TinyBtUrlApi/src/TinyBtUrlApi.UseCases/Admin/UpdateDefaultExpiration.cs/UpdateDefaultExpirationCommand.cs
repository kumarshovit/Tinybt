using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Admin.UpdateDefaultExpiration.cs;

public record UpdateDefaultExpirationCommand(int? DefaultExpirationDays)
    : IRequest<bool>;
