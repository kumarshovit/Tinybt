using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.Entities;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IUrlRepository
{
  Task AddAsync(UrlMapping url);
  Task UpdateAsync(UrlMapping url);

  Task<List<UrlMapping>> GetAllAsync();
  Task<UrlMapping?> GetByIdAsync(int id);
  Task<UrlMapping?> GetByShortCodeAsync(string shortCode);

  Task<bool> ShortCodeExists(string shortCode);

  // TAGS
  Task<List<UrlMapping>> SearchByTagAsync(string tag);
  Task AddTagsAsync(int urlId, List<string> tags);
  Task UpdateTagsAsync(int urlId, List<string> tags);
  Task RemoveTagAsync(int urlId, string tag);
  Task RenameTagAsync(int urlId, string oldTag, string newTag);
}
