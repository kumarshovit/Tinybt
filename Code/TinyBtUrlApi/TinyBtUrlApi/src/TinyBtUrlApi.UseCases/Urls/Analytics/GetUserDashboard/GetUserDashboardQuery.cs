using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
namespace TinyBtUrlApi.UseCases.Urls.Analytics.GetUserDashboard;

public record GetUserDashboardQuery(
    int UserId,
    DateTime From,
    DateTime To)
    : IRequest<DashboardOverviewDto>;
