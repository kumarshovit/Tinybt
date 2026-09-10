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
       .AddAuthConfigs(builder)
       .AddDeveloperApiRateLimiting(builder.Configuration)
       .AddProblemDetails();

// ✅ Environment-based CORS
var MyAllowSpecificOrigins = "AllowFrontend";
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
  {
    var urls = builder.Configuration.GetSection("policyurl:url").Get<string[]>();
    if (urls == null || urls.Length == 0)
    {
      throw new InvalidOperationException(
        "CORS configuration error: 'policyurl:url' is missing or empty in appsettings. " +
        "All environments must explicitly declare allowed origins.");
    }

    policy
          .WithOrigins(urls)
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
         o.DocumentSettings = s =>
         {
           s.Title = "LinkBT API";
           s.Version = "v1";

           // JWT Bearer security scheme (existing authentication — unchanged)
           s.AddAuth("Bearer", new()
           {
             Type = NSwag.OpenApiSecuritySchemeType.Http,
             Scheme = "bearer",
             BearerFormat = "JWT",
             Description = "Standard JWT Bearer token. Obtain from POST /api/auth/login."
           });

           // Developer API Key security scheme (X-API-Key header)
           s.AddAuth("ApiKey", new()
           {
             Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
             Name = "X-API-Key",
             In = NSwag.OpenApiSecurityApiKeyLocation.Header,
             Description = "Developer API key. Format: lbt_live_<random>. Obtain from POST /api/v1/api-keys."
           });
         };
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

// Rate limiter placed AFTER UseAuthentication + UseAuthorization so that
// HttpContext.User (ClaimTypes.NameIdentifier) is available for per-user partition.
app.UseRateLimiter();

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
