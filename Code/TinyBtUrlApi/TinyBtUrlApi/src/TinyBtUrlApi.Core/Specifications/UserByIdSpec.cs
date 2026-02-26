using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Specifications;

public class UserByIdSpec : Specification<User>
{
  public UserByIdSpec(int userId)
  {
    Query.Where(u => u.Id == userId);
  }
}
