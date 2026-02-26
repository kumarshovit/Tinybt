using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Specifications;

public class UserByVerificationTokenSpec : Specification<User>
{
  public UserByVerificationTokenSpec(string token)
  {
    Query.Where(u => u.EmailVerificationToken == token);
  }
}
