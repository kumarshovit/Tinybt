using TinyBtUrlApi.Core.ContributorAggregate;

namespace TinyBtUrlApi.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
