namespace TinyURL.Models
{
    public class UrlTag
    {
        public int UrlMappingId { get; set; }
        public UrlMapping UrlMapping { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
