namespace TinyURL.Models
{
    public class RevokedSession
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime RevokedAt { get; set; }
    }
}
