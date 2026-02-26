using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure;
using TinyBtUrlApi.Infrastructure.Data;
using TinyBtUrlApi.Infrastructure.Email;
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

namespace TinyBtUrlApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(
      this IServiceCollection services,
      Microsoft.Extensions.Logging.ILogger logger,
      WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger);

    // =====================
    // Repositories
    // =====================
    services.AddScoped<IRepository<IpLoginAttempt>, EfRepository<IpLoginAttempt>>();
    services.AddScoped<ITokenRepository, TokenRepository>();
    services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

    // =====================
    // Core Services
    // =====================
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IGoogleAuthService, GoogleAuthService>();

    // =====================
    // Email
    // =====================
    if (builder.Environment.IsDevelopment())
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }
    else
    {
      services.AddScoped<IEmailSender, MimeKitEmailSender>();
    }

    // =====================
    // Handlers
    // =====================
    services.AddScoped<RegisterHandler>();
    services.AddScoped<LoginHandler>();
    services.AddScoped<LogoutHandler>();
    services.AddScoped<ForgotPasswordHandler>();
    services.AddScoped<ResetPasswordHandler>();
    services.AddScoped<VerifyEmailHandler>();
    services.AddScoped<GetProfileHandler>();
    services.AddScoped<UpdateProfileHandler>();
    services.AddScoped<ChangePasswordHandler>();
    services.AddScoped<DeleteAccountHandler>();
    services.AddScoped<GetAllUsersHandler>();
    services.AddScoped<UpdateUserRoleHandler>();
    services.AddScoped<DeleteUserHandler>();
    services.AddScoped<GoogleLoginHandler>();
    logger.LogInformation(
        "{Project} services registered for {Environment}",
        "TinyBtUrlApi",
        builder.Environment.EnvironmentName);

    return services;
  }
}
