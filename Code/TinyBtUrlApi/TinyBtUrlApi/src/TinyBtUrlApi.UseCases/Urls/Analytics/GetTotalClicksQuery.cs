using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

using Mediator;

public record GetTotalClicksQuery() : IRequest<int>;
