using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Specifications;

public class UserByEmailSpec : Specification<User>
{
  public UserByEmailSpec(string email)
  {
    Query.Where(u => u.Email == email);
  }
}
