using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.UseCases.Urls.CreateShortUrl;


public record CreateShortUrlResult
{
  public bool Success { get; init; }
  public string? Message { get; init; }

  public int? Id { get; init; }
  public string? ShortCode { get; init; }
  public string? LongUrl { get; init; }
  public DateTime? ExpirationDate { get; init; }
  public DateTime? CreatedAt { get; init; }
}

