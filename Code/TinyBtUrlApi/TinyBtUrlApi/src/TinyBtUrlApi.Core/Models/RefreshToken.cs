using Ardalis.SharedKernel;
namespace TinyBtUrlApi.Core.Models;

public class RefreshToken : IAggregateRoot
{
  public int Id { get; set; }

  public string Token { get; set; } = string.Empty;

  public DateTime ExpiresAt { get; set; }

  public bool IsRevoked { get; set; }

  public int UserId { get; set; }

  public User? User { get; set; }
}
