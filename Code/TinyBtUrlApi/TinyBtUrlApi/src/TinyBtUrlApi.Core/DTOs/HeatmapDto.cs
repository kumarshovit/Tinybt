namespace TinyBtUrlApi.Core.DTOs;

public class HeatmapDto
{
  public int Day { get; set; } = default!;
  public int Hour { get; set; }
  public int Count { get; set; }
}
