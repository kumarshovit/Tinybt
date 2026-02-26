using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure.Repositories;
using TinyBtUrlApi.Infrastructure.Services;
using TinyBtUrlApi.UseCases.Account.Delete;
using TinyBtUrlApi.UseCases.Account.ForgotPassword;
using TinyBtUrlApi.UseCases.Account.Login;
using TinyBtUrlApi.UseCases.Account.Logout;
using TinyBtUrlApi.UseCases.Account.Profile;
using TinyBtUrlApi.UseCases.Account.Register;
using TinyBtUrlApi.UseCases.Account.ResetPassword;
using TinyBtUrlApi.UseCases.Account.VerifyEmail;
using TinyBtUrlApi.UseCases.Admin.DeleteUser;
using TinyBtUrlApi.UseCases.Admin.GetAllUsers;
using TinyBtUrlApi.UseCases.Admin.UpdateUserRole;
using TinyBtUrlApi.Web.Auth.Create;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.Web.Profile.Delete;
using TinyBtUrlApi.Web.Profile.Read;
using TinyBtUrlApi.Web.Profile.Update;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
       .AddLoggerConfigs();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);

builder.Services
    .AddFastEndpoints()
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
builder.Services.AddScoped<VerifyEmailHandler>();
builder.Services.AddScoped<GetAllUsersHandler>();
builder.Services.AddScoped<UpdateUserRoleHandler>();
builder.Services.AddScoped<DeleteUserHandler>();
builder.Services.AddScoped<GetProfileHandler>();
builder.Services.AddScoped<UpdateProfileHandler>();
builder.Services.AddScoped<ChangePasswordHandler>();
builder.Services.AddScoped<DeleteAccountHandler>();
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


app.UseAuthentication();
app.UseAuthorization();

// ✅ require
app.UseFastEndpoints();
app.MapForgotPasswordEndpoint();
app.MapLogoutEndpoint();
app.MapGoogleLoginEndpoint();
app.MapResetPasswordEndpoint();
app.MapGetProfileEndpoint();
app.MapUpdateNameEndpoint();
app.MapChangePasswordEndpoint();
app.MapDeleteAccountEndpoint();
app.UseSwaggerGen();      // ✅ required for swagger

app.Run();

public partial class Program { }
