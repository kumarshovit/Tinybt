using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Web.Configurations;

public static class AuthConfigs
{
  public static IServiceCollection AddAuthConfigs(
      this IServiceCollection services,
      WebApplicationBuilder builder)
  {
    services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role
          };

          options.Events = new JwtBearerEvents
          {
            OnTokenValidated = async context =>
            {
              var repo = context.HttpContext.RequestServices
                      .GetRequiredService<ITokenRepository>();

              var jwt = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
              var rawToken = jwt?.RawData;

              if (rawToken != null &&
                      await repo.IsTokenRevoked(rawToken))
              {
                context.Fail("Token revoked.");
              }
            }
          };
        });

    services.AddAuthorization();

    return services;
  }
}
