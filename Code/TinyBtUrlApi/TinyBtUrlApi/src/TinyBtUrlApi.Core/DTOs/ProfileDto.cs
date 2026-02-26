namespace TinyBtUrlApi.Core.DTOs;

public class ProfileDto
{
  public string? Email { get; set; } = default!;
  public string? FullName { get; set; } = default!;
  public DateTime CreatedAt { get; set; }
}
