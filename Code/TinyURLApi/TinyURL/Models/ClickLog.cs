namespace TinyURL.Models
{
    public class ClickLog
    {
        public int Id { get; set; }

        public string ShortCode { get; set; }

        public DateTime ClickedAt { get; set; }

        public string? Referrer { get; set; }

        public string? Country { get; set; }

        public string? DeviceType { get; set; }

        public string? Browser { get; set; }

        public string? OS { get; set; }

        public string? DeviceLanguage { get; set; }
    }
}
