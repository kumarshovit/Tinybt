using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Register;

public class RegisterHandler
{
  private readonly IRepository<User> _repository;

  public RegisterHandler(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task<string> Handle(RegisterQuery query)
  {
    var dto = query.Dto;

    var existingUser = await _repository.FirstOrDefaultAsync(
        new UserByEmailSpec(dto.Email)
    );

    if (existingUser != null)
      return "Email already exists.";

    var user = new User
    {
      Email = dto.Email.ToLower(),
      PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
      CreatedAt = DateTime.UtcNow
    };

    await _repository.AddAsync(user);

    return "Registration successful.";
  }
}
