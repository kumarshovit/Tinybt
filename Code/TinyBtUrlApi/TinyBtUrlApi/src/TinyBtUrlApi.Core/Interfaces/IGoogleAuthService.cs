using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IGoogleAuthService
{
  Task<User?> ValidateTokenAsync(string idToken);
}
