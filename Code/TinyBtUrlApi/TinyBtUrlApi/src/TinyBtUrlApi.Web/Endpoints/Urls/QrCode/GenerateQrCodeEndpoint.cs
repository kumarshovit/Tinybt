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
            await Send.NotFoundAsync(ct);
            return;
        }

        var urlData = await _urlRepository.GetByShortCodeAsync(shortCode);
        if (urlData == null || urlData.IsDeleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var request = HttpContext.Request;

        // Fix 1: On localhost use the actual request host so the QR encodes the right URL.
        // In production, prefer the configured short URL domain, then fall back to proxy headers.
        string backendBaseUrl;
        if (request.Host.Host.Contains("localhost"))
        {
            backendBaseUrl = $"{request.Scheme}://{request.Host}";
        }
        else
        {
            var configDomain = _configuration["BaseUrl:ShortUrlDomain"] ?? _configuration["BaseUrl:Domain"];
            if (!string.IsNullOrWhiteSpace(configDomain))
            {
                backendBaseUrl = configDomain.TrimEnd('/');
            }
            else
            {
                var fwdHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
                var host = !string.IsNullOrWhiteSpace(fwdHost) ? fwdHost.Split(',')[0].Trim() : request.Host.ToString();

                var fwdProto = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
                var scheme = !string.IsNullOrWhiteSpace(fwdProto) ? fwdProto.Split(',')[0].Trim() : request.Scheme;

                backendBaseUrl = $"{scheme}://{host}";
            }
        }

        var qrCodeTargetUrl = $"{backendBaseUrl}/q/{shortCode}";

        var imageBytes = _qrCodeService.GenerateQrCode(qrCodeTargetUrl);

        // Fix 2: Use FastEndpoints' SendBytesAsync instead of writing directly to the body stream.
        // Writing to Body.WriteAsync manually caused FastEndpoints to override the response with 204 No Content.
        await Send.BytesAsync(imageBytes, fileName: null, contentType: "image/png", cancellation: ct);
    }
}
