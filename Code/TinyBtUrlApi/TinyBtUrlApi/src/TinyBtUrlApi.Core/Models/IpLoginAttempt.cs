using Ardalis.SharedKernel;

namespace TinyBtUrlApi.Core.Models;

public class IpLoginAttempt : IAggregateRoot
{
  public int Id { get; set; }

  public string IpAddress { get; set; } = string.Empty;

  public int AttemptCount { get; set; }

  public DateTime LastAttemptAt { get; set; }

  public DateTime? BlockedUntil { get; set; }
}
