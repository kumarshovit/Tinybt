namespace TinyURL.Models
{
    public class UrlMapping
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortCode { get; set; }
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }


        public ICollection<UrlTag> UrlTags { get; set; }
    }
}