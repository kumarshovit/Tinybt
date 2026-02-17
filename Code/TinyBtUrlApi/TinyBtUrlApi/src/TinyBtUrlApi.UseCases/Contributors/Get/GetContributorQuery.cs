using TinyBtUrlApi.Core.ContributorAggregate;

namespace TinyBtUrlApi.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
