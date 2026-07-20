using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Options;
using TinyBtUrlApi.Core.Services;
using TinyBtUrlApi.Infrastructure.Data;
using TinyBtUrlApi.Infrastructure.Repositories;
using TinyBtUrlApi.Infrastructure.Services;

namespace TinyBtUrlApi.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    // Try to get connection strings in order of priority:
    // 1. "cleanarchitecture" - provided by Aspire when using .WithReference(cleanArchDb)
    // 2. "DefaultConnection" - traditional SQL Server connection
    // 3. "SqliteConnection" - fallback to SQLite
    string? connectionString = config.GetConnectionString("cleanarchitecture")
                               ?? config.GetConnectionString("DefaultConnection") 
                               ?? config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();
    services.AddScoped<ISettingsRepository, SettingsRepository>();
    services.AddScoped<IContactRepository, ContactRepository>();
    services.AddScoped<IContactEmailService, ContactEmailService>();

    services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
      
      // Use SQL Server if Aspire or DefaultConnection is available, otherwise use SQLite
      if (config.GetConnectionString("cleanarchitecture") != null || 
          config.GetConnectionString("DefaultConnection") != null)
      {
        options.UseSqlServer(connectionString);
      }
      else
      {
        options.UseSqlite(connectionString);
      }
      
      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
           .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));


    services.AddScoped<IUrlRepository, UrlRepository>();
    services.AddScoped<ShortCodeService>();
    services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
    // URL Security Validations
    services.AddMemoryCache();
    services.AddScoped<IUrlFormatValidator, UrlFormatValidator>();
    services.AddScoped<IDnsLookupService, DnsLookupService>();
    //services.AddHttpClient<IHttpsValidationService, HttpsValidationService>();
    services.AddScoped<IUrlSecurityValidator, UrlSecurityValidator>();
    services.AddHttpClient<ICaptchaService, TurnstileCaptchaService>();
    
    services.Configure<GoogleSafeBrowsingOptions>(config.GetSection("GoogleSafeBrowsing"));
    services.AddHttpClient<IGoogleSafeBrowsingService, GoogleSafeBrowsingService>();

    logger.LogInformation("{Project} services registered", "Infrastructure");


    return services;
  }
}
