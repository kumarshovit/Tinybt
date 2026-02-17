using TinyBtUrlApi.Core.ContributorAggregate;

namespace TinyBtUrlApi.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
