using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.Core.Specifications;

public class UserByMicrosoftIdSpec : Specification<User>
{
  public UserByMicrosoftIdSpec(string microsoftId)
  {
    Query.Where(u => u.MicrosoftId == microsoftId);
  }
}

