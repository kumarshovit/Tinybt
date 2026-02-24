using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Account.Logout;

public class LogoutHandler
{
  private readonly ITokenRepository _tokenRepository;

  public LogoutHandler(ITokenRepository tokenRepository)
  {
    _tokenRepository = tokenRepository;
  }

  public async Task HandleAsync(LogoutQuery query)
  {
    // Revoke Access Token
    await _tokenRepository.AddRevokedAccessTokenAsync(query.AccessToken);

    // Revoke Refresh Token
    var refreshToken = await _tokenRepository
        .GetRefreshTokenAsync(query.RefreshToken);

    if (refreshToken != null)
    {
      refreshToken.IsRevoked = true;
    }

    await _tokenRepository.SaveChangesAsync();
  }
}
