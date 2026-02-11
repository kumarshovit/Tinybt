namespace TinyUrlApi.Models
{
    public class UrlMapping
    {
        public int Id { get; set; }
        public string ShortCode { get; set; }
        public string LongUrl { get; set; }
        public int ClickCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
