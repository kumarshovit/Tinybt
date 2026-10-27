using System;
using System.Collections.Generic;
using System.Text;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Models;

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
  Task<int> GetTotalClicksAsync(CancellationToken cancellationToken);
  Task<List<ClickOverTimeDto>> GetClicksOverTimeAsync(
    DateTime startDate,
    DateTime endDate,
    string viewType,
    CancellationToken cancellationToken);

  //Task<List<ClicksByBrowserDto>> GetClicksByBrowserAsync(
  //  DateTime startDate,
  //  DateTime endDate,
  //  CancellationToken cancellationToken);

  Task<List<ClicksByBrowserDto>> GetClicksByBrowserAsync(
    DateTime? startDate,
    DateTime? endDate,
    CancellationToken cancellationToken);

  Task<List<ClicksByCountryDto>> GetClicksByCountryAsync(
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken);

  Task<List<TopUrlDto>> GetTopUrlsAsync(
    int topCount,
    CancellationToken cancellationToken);

  //Task<List<ClicksByDeviceLanguageDto>>
  //  GetClicksByDeviceLanguageAsync(CancellationToken cancellationToken);

  Task<List<ClicksByDeviceLanguageDto>> GetClicksByDeviceLanguageAsync(
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken);

  //Task<List<ClicksByOsDto>> GetClicksByOsAsync(
  //  CancellationToken cancellationToken);

  Task<List<ClicksByOsDto>> GetClicksByOsAsync(
    DateTime startDate,
    DateTime endDate,
    CancellationToken cancellationToken);

  Task<List<ClicksByDeviceTypeDto>> GetClicksByDeviceTypeAsync(
    DateTime? startDate,
    DateTime? endDate,
    CancellationToken cancellationToken);

  Task<List<ClickLog>> GetClickLogsByShortCodeAsync(
    string shortCode,
    CancellationToken ct);

  Task LogClickAsync(ClickLog clickLog, CancellationToken cancellationToken);

  Task<List<ClickOverTimeDto>> GetLinkClicksOverTimeAsync(
    string shortCode,
    DateTime startDate,
    DateTime endDate,
    string viewType,
    CancellationToken cancellationToken);


  Task<List<PopularDaysTimesDto>> GetPopularDaysTimesAsync(
    DateTime? startDate,
    DateTime? endDate,
    CancellationToken ct);

  // Safety Flagging & Verification
  Task FlagUrlAsync(int urlId, string reason, CancellationToken ct = default);
  Task UnflagUrlAsync(int urlId, CancellationToken ct = default);
  Task UpdateLastSafetyCheckAsync(int urlId, CancellationToken ct = default);
  Task<List<UrlMapping>> GetUrlsForSafetyRecheckAsync(int batchSize, CancellationToken ct = default);

  // Screenshots
  Task<UrlMapping?> GetExistingScreenshotAsync(string longUrlHash, CancellationToken ct = default);
  Task UpdateScreenshotStatusAsync(int id, ScreenshotStatus status, string? path, string? scanUuid, CancellationToken ct = default);
  Task<List<UrlMapping>> GetPendingScreenshotsAsync(int limit = 50, CancellationToken ct = default);
}
