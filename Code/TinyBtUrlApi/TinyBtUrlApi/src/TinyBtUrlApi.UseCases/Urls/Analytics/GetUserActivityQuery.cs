using System;
using System.Collections.Generic;
using System.Text;

using Mediator;
using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public sealed record GetUserActivityQuery(int UserId)
    : IRequest<List<UserActivityDto>>;
