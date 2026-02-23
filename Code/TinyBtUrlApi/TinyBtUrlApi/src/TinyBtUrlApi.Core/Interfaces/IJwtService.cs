namespace TinyBtUrlApi.Core.Interfaces;

using TinyBtUrlApi.Core.Models;

public interface IJwtService
{
  (string token, DateTime expires) Generate(User user);
}
