using Ardalis.SharedKernel;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure;
using TinyBtUrlApi.Infrastructure.Data;
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
    services.AddInfrastructureServices(builder.Configuration, logger);
    services.AddScoped<LoginHandler>();
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<RegisterHandler>();
    services.AddScoped<LogoutHandler>();
    services.AddScoped<ForgotPasswordHandler>();
    services.AddScoped<ResetPasswordHandler>();
    services.AddScoped<VerifyEmailHandler>();
    services.AddScoped<DeleteAccountHandler>();
    services.AddScoped<GetProfileHandler>();
    services.AddScoped<UpdateProfileHandler>();
    services.AddScoped<DeleteUserHandler>();
    services.AddScoped<GetAllUsersHandler>();
    services.AddScoped<UpdateUserRoleHandler>();
    // Email
    services.AddScoped<IEmailSender, EmailService>();
  
    // Token Repository
    services.AddScoped<ITokenRepository, TokenRepository>();
    // Password Reset Repository
    services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
