using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IMicrosoftAuthService
{
  Task<User?> ValidateTokenAsync(string idToken);
}

