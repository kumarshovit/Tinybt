using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Specifications;

namespace TinyBtUrlApi.UseCases.Account.Profile;

public class ChangePasswordHandler
{
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<RefreshToken> _refreshTokenRepository;

  public ChangePasswordHandler(
      IRepository<User> userRepository,
      IRepository<RefreshToken> refreshTokenRepository)
  {
    _userRepository = userRepository;
    _refreshTokenRepository = refreshTokenRepository;
  }

  public async Task<(bool Success, string Message)> Handle(ChangePasswordCommand command)
  {
    var spec = new UserByIdSpec(command.UserId);
    var user = await _userRepository.FirstOrDefaultAsync(spec);

    if (user == null)
      return (false, "User not found.");

    if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
      return (false, "Current password incorrect.");

    // 🔐 Hash new password
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword);

    await _userRepository.UpdateAsync(user);

    // 🔥 Revoke ALL refresh tokens for this user
    var refreshTokens = await _refreshTokenRepository.ListAsync(
        new RefreshTokensByUserSpec(user.Id));

    foreach (var token in refreshTokens)
    {
      token.IsRevoked = true;
      await _refreshTokenRepository.UpdateAsync(token);
    }

    return (true, "Password changed successfully. All sessions logged out.");
  }
}
