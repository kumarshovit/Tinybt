namespace TinyBtUrlApi.Core.Options;

public class AbstractEmailValidationOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://emailvalidation.abstractapi.com/v1/";
    public int TimeoutSeconds { get; set; } = 3;
    public bool BlockDisposable { get; set; } = true;
}
