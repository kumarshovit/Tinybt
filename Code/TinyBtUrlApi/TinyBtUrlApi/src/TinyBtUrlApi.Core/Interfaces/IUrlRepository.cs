using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Core.Interfaces;

internal interface IUrlRepository
{
  Task AddAsync(UrlMapping url);
  Task<UrlMapping?> GetByShortCodeAsync(string shortCode);
  Task<List<UrlMapping>> GetAllAsync();
  Task SaveChangesAsync();
}
