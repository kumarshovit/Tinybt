using Ardalis.Specification;
using TinyBtUrlApi.Core.Models;

public class IpLoginAttemptByIpSpec : Specification<IpLoginAttempt>
{
  public IpLoginAttemptByIpSpec(string ip)
  {
    Query.Where(x => x.IpAddress == ip);
  }
}
