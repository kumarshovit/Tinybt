using TinyBtUrlApi.Core.DTOs;

namespace TinyBtUrlApi.UseCases.Account.Register;

public class RegisterQuery
{
  public RegisterDto Dto { get; }

  public RegisterQuery(RegisterDto dto)
  {
    Dto = dto;
  }
}
