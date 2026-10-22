using System;

namespace TinyBtUrlApi.Core.Entities;

public class UrlReport
{
    public int Id { get; set; }
    public string ShortCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;       // "phishing", "malware", "scam", "other"
    public string? Details { get; set; }                     // User's description
    public string? ReporterIp { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public bool IsReviewed { get; set; } = false;
    public string? AdminNotes { get; set; }
}
