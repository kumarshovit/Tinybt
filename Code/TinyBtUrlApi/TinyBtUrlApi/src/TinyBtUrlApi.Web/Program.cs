using Microsoft.AspNetCore.DataProtection.Repositories;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure.Data;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using TinyBtUrlApi.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
       .AddLoggerConfigs();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services
       .AddOptionConfigs(builder.Configuration, startupLogger, builder)
       .AddServiceConfigs(startupLogger, builder)
       .AddAuthConfigs(builder)
       .AddCorsConfigs();
//builder.Services.AddScoped<IUrlRepository, UrlRepository>();
//builder.Services.AddScoped<ShortCodeService>();

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
    policy =>
    {
      policy.WithOrigins("http://localhost:5173") // frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddMediator(options =>
{
  options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services
       .AddFastEndpoints()
       .SwaggerDocument(o => o.ShortSchemaNames = true);

var app = builder.Build();



app.UseCorsConfigs();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpointConfigs();


app.Run();

public partial class Program { }
