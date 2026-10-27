namespace TinyBtUrlApi.Core.Options;

public class ScreenshotOptions
{
    public string StoragePath { get; set; } = "wwwroot/screenshots";
    public int MaxSizeBytes { get; set; } = 5242880; // 5 MB
    public int DownloadTimeoutSeconds { get; set; } = 15;
}
