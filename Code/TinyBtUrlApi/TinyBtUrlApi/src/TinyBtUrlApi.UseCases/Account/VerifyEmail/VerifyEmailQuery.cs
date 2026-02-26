namespace TinyBtUrlApi.UseCases.Account.VerifyEmail;

public class VerifyEmailQuery
{
  public string Token { get; }

  public VerifyEmailQuery(string token)
  {
    Token = token;
  }
}
