namespace TinyBtUrlApi.Core.Interfaces;

using TinyBtUrlApi.Core.Models;

public interface IJwtService
{
  (string Token, DateTime Expires) Generate(User user);
}
