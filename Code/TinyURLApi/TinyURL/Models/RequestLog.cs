namespace TinyURL.Models
{
    public class RequestLog
    {
        public int Id { get; set; }

        public string Endpoint { get; set; }
        public string Method { get; set; }
        
        public string? ShortCode { get; set; }   // important for TinyURL redirect

        public string Headers { get; set; }     // JSON

        public string? RawBody { get; set; }   // ⭐ NEW
        public string? QueryString { get; set; }   // ⭐ NEW
        public string? RouteValues { get; set; }   // ⭐ NEW

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public int StatusCode { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
