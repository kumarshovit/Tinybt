namespace TinyBtUrlApi.Core.Configuration;

public class UrlSecurityOptions
{
    public const string SectionName = "UrlSecurity";

    public int MaxUrlLength { get; set; } = 2048;
    public bool AllowHttp { get; set; } = false;
    public bool PreferHttps { get; set; } = false;
    public int DnsTimeoutMilliseconds { get; set; } = 2000;
    public bool EnableDnsValidation { get; set; } = true;
    public int HttpsTimeoutMilliseconds { get; set; } = 2000;
    public int MaxRedirectHops { get; set; } = 5;
    public int RedirectTimeoutMilliseconds { get; set; } = 3000;
    public bool EnableRedirectChainValidation { get; set; } = true;
}
