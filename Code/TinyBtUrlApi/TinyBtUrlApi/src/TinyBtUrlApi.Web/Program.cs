using Microsoft.AspNetCore.Diagnostics;
using TinyBtUrlApi.Web.Auth.Create;
using TinyBtUrlApi.Web.Configurations;
using TinyBtUrlApi.Web.Profile.Delete;
using TinyBtUrlApi.Web.Profile.Read;
using TinyBtUrlApi.Web.Profile.Update;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// Logging & Defaults
// ---------------------------
builder.AddServiceDefaults()
       .AddLoggerConfigs();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

// ---------------------------
// Service Registrations
// ---------------------------
builder.Services
       .AddOptionConfigs(builder.Configuration, startupLogger, builder)
       .AddServiceConfigs(startupLogger, builder)
       .AddAuthConfigs(builder);

// ✅ Environment-based CORS
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    var allowedOrigins = builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>();

    policy.WithOrigins(allowedOrigins!)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});

// Mediator
builder.Services.AddMediator(options =>
{
  options.ServiceLifetime = ServiceLifetime.Scoped;
});

// FastEndpoints + Swagger
builder.Services
       .AddFastEndpoints()
       .SwaggerDocument(o =>
       {
         o.ShortSchemaNames = true;
       });

// ---------------------------
// Build App
// ---------------------------
var app = builder.Build();

// ---------------------------
// Middleware Pipeline
// ---------------------------
app.UseExceptionHandler(errorApp =>
{
  errorApp.Run(async context =>
  {
    context.Response.StatusCode = 400;
    context.Response.ContentType = "application/json";

    var error = context.Features.Get<IExceptionHandlerFeature>();
    if (error != null)
    {
      await context.Response.WriteAsJsonAsync(new
      {
        message = error.Error.Message
      });
    }
  });
});

// ✅ CORS before Authentication
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.MapGetProfileEndpoint();
app.MapChangePasswordEndpoint();
app.MapUpdateNameEndpoint();
app.MapDeleteAccountEndpoint();
app.MapGoogleLoginEndpoint();
app.MapForgotPasswordEndpoint();
app.MapLogoutEndpoint();
app.MapResetPasswordEndpoint();

app.Run();

public partial class Program { }
