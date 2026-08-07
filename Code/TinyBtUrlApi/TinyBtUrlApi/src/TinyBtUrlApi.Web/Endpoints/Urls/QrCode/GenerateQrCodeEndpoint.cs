using FastEndpoints;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Web.Endpoints.Urls.QrCode;

public class GenerateQrCodeEndpoint : EndpointWithoutRequest
{
    private readonly IQrCodeService _qrCodeService;
    private readonly IUrlRepository _urlRepository;
    private readonly IConfiguration _configuration;

    public GenerateQrCodeEndpoint(
        IQrCodeService qrCodeService,
        IUrlRepository urlRepository,
        IConfiguration configuration)
    {
        _qrCodeService = qrCodeService;
        _urlRepository = urlRepository;
        _configuration = configuration;
    }

    public override void Configure()
    {
        Get("/api/qrcode/{shortCode}");
        AllowAnonymous();
        Description(x => x.WithTags("Url Management"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var urlData = await _urlRepository.GetByShortCodeAsync(shortCode);
        if (urlData == null || urlData.IsDeleted)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var request = HttpContext.Request;
        var backendBaseUrl = $"{request.Scheme}://{request.Host}";
        var qrCodeTargetUrl = $"{backendBaseUrl}/q/{shortCode}";

        var imageBytes = _qrCodeService.GenerateQrCode(qrCodeTargetUrl);
        
        HttpContext.Response.ContentType = "image/png";
        await HttpContext.Response.Body.WriteAsync(imageBytes, 0, imageBytes.Length, ct);
    }
}
