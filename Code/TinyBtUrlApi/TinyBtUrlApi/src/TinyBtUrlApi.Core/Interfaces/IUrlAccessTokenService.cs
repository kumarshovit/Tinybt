using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IUrlAccessTokenService
{
    string Protect(string shortCode, string source);
    AccessTokenPayload? TryUnprotect(string token, string expectedShortCode);
}
