using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IPasswordResetRepository
{
  Task AddAsync(PasswordResetToken token);
  Task<PasswordResetToken?> GetByTokenAsync(string token);
  Task SaveChangesAsync();
}
