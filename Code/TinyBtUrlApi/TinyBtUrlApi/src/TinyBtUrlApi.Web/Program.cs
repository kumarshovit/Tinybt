using System.Text;
using Microsoft.IdentityModel.Tokens;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure.Repositories;
using TinyBtUrlApi.Infrastructure.Services;
using TinyBtUrlApi.UseCases.Account.Login;
using TinyBtUrlApi.UseCases.Account.Logout;
using TinyBtUrlApi.UseCases.Account.Register;
using TinyBtUrlApi.Web.Auth.Create;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.UseCases.Account.ForgotPassword;
using TinyBtUrlApi.UseCases.Account.ResetPassword;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
       .AddLoggerConfigs();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
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
              Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
      };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<GoogleLoginHandler>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IEmailSender, EmailService>();
builder.Services.AddScoped<LogoutHandler>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<ForgotPasswordHandler>();
builder.Services.AddScoped<ResetPasswordHandler>();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
      });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Middleware first
await app.UseAppMiddlewareAndSeedDatabase();
app.UseAuthentication();
app.UseAuthorization();
// Then map endpoints
app.MapGoogleLoginEndpoint();
app.MapLogoutEndpoint();
app.MapForgotPasswordEndpoint();
app.MapResetPasswordEndpoint();
app.MapDefaultEndpoints();
app.Run();

public partial class Program { }
