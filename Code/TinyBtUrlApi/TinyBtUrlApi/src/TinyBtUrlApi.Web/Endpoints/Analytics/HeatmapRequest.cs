public class HeatmapRequest
{
  public DateTime From { get; set; }
  public DateTime To { get; set; }

  public int? UserId { get; set; }  // ✅ for admin
}
