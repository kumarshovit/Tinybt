using TinyBtUrlApi.Core.ContributorAggregate;

namespace TinyBtUrlApi.UseCases.Contributors;
public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
