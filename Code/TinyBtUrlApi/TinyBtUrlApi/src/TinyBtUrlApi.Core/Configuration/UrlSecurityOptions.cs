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
    public int MaxAllowedRedirects { get; set; } = 2;
    public int MaxRedirectHops
    {
        get => MaxAllowedRedirects;
        set => MaxAllowedRedirects = value;
    }
    public int RedirectTimeoutMilliseconds { get; set; } = 3000;
    public bool EnableRedirectChainValidation { get; set; } = true;
}
