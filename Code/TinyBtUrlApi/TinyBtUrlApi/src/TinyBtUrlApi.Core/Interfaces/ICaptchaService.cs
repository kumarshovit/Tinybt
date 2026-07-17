using System.Threading;
using System.Threading.Tasks;

namespace TinyBtUrlApi.Core.Interfaces;

public interface ICaptchaService
{
    Task<bool> VerifyTokenAsync(string token, string? remoteIp = null, CancellationToken ct = default);
}
