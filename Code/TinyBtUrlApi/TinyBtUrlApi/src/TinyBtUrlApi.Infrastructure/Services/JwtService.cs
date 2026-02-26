using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Services;

public class JwtService : IJwtService
{
  private readonly IConfiguration _config;

  public JwtService(IConfiguration config)
  {
    _config = config;
  }

  public (string Token, DateTime Expires) Generate(User user)
  {
    var jwt = _config.GetSection("Jwt");

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwt["Key"]!)
    );

    var creds = new SigningCredentials(
        key, SecurityAlgorithms.HmacSha256
    );

    // 🔥 Configurable expiration
    var accessMinutes = double.Parse(jwt["AccessTokenMinutes"]!);

    var expires = DateTime.UtcNow.AddMinutes(accessMinutes);

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new Claim(ClaimTypes.Email, user.Email ?? ""),
      new Claim(ClaimTypes.Role, user.Role ?? "User")
    };

    var token = new JwtSecurityToken(
      issuer: jwt["Issuer"],
      audience: jwt["Audience"],
      claims: claims,
      expires: expires,
      signingCredentials: creds
    );

    return (
      new JwtSecurityTokenHandler().WriteToken(token),
      expires
    );
  }
}
