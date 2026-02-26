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

builder.Services
       .AddFastEndpoints()
       .SwaggerDocument(o => o.ShortSchemaNames = true);

var app = builder.Build();

app.UseCorsConfigs();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpointConfigs();

app.Run();

public partial class Program { }
