using System;
using System.Collections.Generic;
using System.Text;
using Mediator;



namespace TinyBtUrlApi.UseCases.Admin.GetDefaultExpiration;

public record GetDefaultExpirationQuery() : IRequest<int?>;
