using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Profile;

public class GetProfileHandler
{
  private readonly IRepository<User> _repository;

  public GetProfileHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<ProfileDto?> Handle(GetProfileQuery query)
  {
    var spec = new UserByIdSpec(query.UserId);
    var user = await _repository.FirstOrDefaultAsync(spec);

    if (user == null) return null;

    return new ProfileDto
    {
      Email = user.Email,
      FullName = user.FullName,
      CreatedAt = user.CreatedAt
    };
  }
}
