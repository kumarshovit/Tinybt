namespace TinyURL.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UrlTag> UrlTags { get; set; }
    }
}
