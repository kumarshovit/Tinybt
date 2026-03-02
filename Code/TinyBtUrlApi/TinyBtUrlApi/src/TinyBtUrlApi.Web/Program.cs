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
       .AddCorsConfigs();

// Custom CORS policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.WithOrigins("http://164.52.216.107:9007")
                .AllowAnyHeader()
                .AllowAnyMethod();
      });
});
//builder.Services.AddCors(options =>
//{
//  options.AddPolicy("AllowFrontend",
//      policy =>
//      {
//        policy.WithOrigins("http://localhost:5174")
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//      });
//});

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
app.UseCorsConfigs();
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
// ⚠️ IMPORTANT: Chain these


app.Run();

public partial class Program { }
