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
       .AddCorsConfigs();

// Custom CORS policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
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
app.UseCorsConfigs();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapGetProfileEndpoint();
app.MapChangePasswordEndpoint();
app.MapUpdateNameEndpoint();
app.MapDeleteAccountEndpoint();
app.MapGoogleLoginEndpoint();
app.MapForgotPasswordEndpoint();
app.MapLogoutEndpoint();
app.MapResetPasswordEndpoint();
// ⚠️ IMPORTANT: Chain these
app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();

public partial class Program { }
