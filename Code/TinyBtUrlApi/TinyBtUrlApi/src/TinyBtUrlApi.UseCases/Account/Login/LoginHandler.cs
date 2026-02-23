using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Specifications;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Account.Login;

public class LoginHandler
{
  private readonly IRepository<User> _repository;
  private readonly IJwtService _jwtService;   // ✅ interface use karo

  public LoginHandler(IRepository<User> repository, IJwtService jwtService)
  {
    _repository = repository;
    _jwtService = jwtService;
  }

  public async Task<object> Handle(LoginQuery query)
  {
    var dto = query.Dto;
    var email = dto.Email!.Trim().ToLower();

    var user = await _repository.FirstOrDefaultAsync(
      new UserByEmailSpec(email)
    );

    if (user == null ||
        !BCrypt.Net.BCrypt.Verify(dto.Password!, user.PasswordHash))
    {
      return new { message = "Invalid email or password." };
    }

    var result = _jwtService.Generate(user);

    return new
    {
      accessToken = result.token,
      expires = result.expires
    };
  }
}
