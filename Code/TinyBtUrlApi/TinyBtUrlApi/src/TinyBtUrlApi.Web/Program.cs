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
var MyAllowSpecificOrigins = "AllowFrontend";
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
  {
    var urls = builder.Configuration.GetSection("policyurl:url").Get<string[]>();
    policy
          .WithOrigins(urls!)
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
         o.AutoTagPathSegmentIndex = 0;
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
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";

    var error = context.Features.Get<IExceptionHandlerFeature>();
    if (error != null)
    {
      var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
      logger.LogError(error.Error, "Unhandled exception");

      await context.Response.WriteAsJsonAsync(new
      {
        message = "An unexpected error occurred."
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
