using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.Analytics;

public sealed class GetDashboardOverviewHandler(
    IAnalyticsRepository repository)
    : IQueryHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
{
  //public async ValueTask<DashboardOverviewDto> Handle(
  //    GetDashboardOverviewQuery request,
  //    CancellationToken cancellationToken)
  //{
  //  var from = request.From ?? DateTime.UtcNow.AddDays(-30);
  //  var to = request.To ?? DateTime.UtcNow;

  //  var totalClicks = await repository
  //      .GetTotalClicksAsync(from, to, cancellationToken);

  //  var uniqueVisitors = await repository
  //      .GetUniqueVisitorsAsync(from, to, cancellationToken);

  //  var totalUrls = await repository
  //      .GetTotalUrlsAsync(cancellationToken);

  //  var totalTags = await repository
  //      .GetTotalTagsAsync(cancellationToken);

  //  return new DashboardOverviewDto
  //  {
  //    TotalClicks = totalClicks,
  //    UniqueVisitors = uniqueVisitors,
  //    TotalUrls = totalUrls,
  //    TotalTags = totalTags,
  //    From = from,
  //    To = to
  //  };
  //}

  public async ValueTask<DashboardOverviewDto> Handle(
    GetDashboardOverviewQuery request,
    CancellationToken cancellationToken)
  {
    var from = request.From ?? DateTime.UtcNow.AddDays(-30);
    var to = request.To ?? DateTime.UtcNow;

    var totalClicks = await repository
        .GetTotalClicksAsync(from, to, cancellationToken);

    var uniqueVisitors = await repository
        .GetUniqueVisitorsAsync(from, to, cancellationToken);

    var totalUrls = await repository
        .GetTotalUrlsAsync(cancellationToken);

    var totalTags = await repository
        .GetTotalTagsAsync(cancellationToken);

    var activeLinks = await repository
        .GetActiveLinksAsync(cancellationToken);

    var expiredLinks = await repository
        .GetExpiredLinksAsync(cancellationToken);

    return new DashboardOverviewDto
    {
      TotalClicks = totalClicks,
      UniqueVisitors = uniqueVisitors,
      TotalUrls = totalUrls,
      TotalTags = totalTags,
      ActiveLinks = activeLinks,
      ExpiredLinks = expiredLinks,
      From = from,
      To = to
    };
  }
}
