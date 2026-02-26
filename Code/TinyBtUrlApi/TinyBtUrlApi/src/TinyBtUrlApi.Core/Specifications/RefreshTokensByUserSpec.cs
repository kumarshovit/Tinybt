using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

public class RefreshTokensByUserSpec : Specification<RefreshToken>
{
  public RefreshTokensByUserSpec(int userId)
  {
    Query.Where(t => t.UserId == userId && !t.IsRevoked);
  }
}
