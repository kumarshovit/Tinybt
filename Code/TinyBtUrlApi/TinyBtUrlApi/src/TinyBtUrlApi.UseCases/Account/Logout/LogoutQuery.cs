namespace TinyBtUrlApi.UseCases.Account.Logout;

public record LogoutQuery(string AccessToken, string RefreshToken);
