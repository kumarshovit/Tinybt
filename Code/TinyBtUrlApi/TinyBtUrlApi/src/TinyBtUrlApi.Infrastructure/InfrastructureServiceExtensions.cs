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
    services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
    services.AddScoped<IApiKeyGeneratorService, ApiKeyGeneratorService>();

    // URL Security Validations
    services.AddMemoryCache();
    services.AddScoped<IUrlFormatValidator, UrlFormatValidator>();
    services.AddScoped<IDnsLookupService, DnsLookupService>();
    //services.AddHttpClient<IHttpsValidationService, HttpsValidationService>();
    services.AddScoped<IUrlSecurityValidator, UrlSecurityValidator>();
    services.AddHttpClient<ICaptchaService, TurnstileCaptchaService>();
    
    // Security & Abuse Protection
    services.AddScoped<IUrlAccessTokenService, TinyBtUrlApi.Infrastructure.Security.UrlAccessTokenService>();
    services.AddScoped<IPasswordRateLimiter, TinyBtUrlApi.Infrastructure.Security.PasswordRateLimiter>();
    services.Configure<AbuseProtectionOptions>(config.GetSection("AbuseProtection"));
    services.AddSingleton<IUrlAbuseTrackingService, TinyBtUrlApi.Infrastructure.Security.UrlAbuseTrackingService>();

    services.Configure<GoogleSafeBrowsingOptions>(config.GetSection("GoogleSafeBrowsing"));
    services.AddHttpClient<IGoogleSafeBrowsingService, GoogleSafeBrowsingService>();

    // URLhaus (abuse.ch) Malware URL Database
    services.Configure<UrlhausOptions>(config.GetSection("Urlhaus"));
    services.AddSingleton<UrlhausFeedStore>();
    services.AddHostedService<UrlhausBackgroundService>();
    services.AddScoped<IUrlhausService, UrlhausService>();
    services.AddHttpClient("UrlhausFeed");

    // URL Safety Orchestrator (combines URLhaus + GSB for quick rechecks)
    services.Configure<SafetyRecheckOptions>(config.GetSection("SafetyRecheck"));
    services.AddScoped<IUrlSafetyOrchestrator, UrlSafetyOrchestrator>();

    // Background Periodic URL Safety Recheck Job
    services.Configure<BackgroundRecheckOptions>(config.GetSection("BackgroundRecheck"));
    services.AddHostedService<UrlSafetyRecheckBackgroundService>();

    // Cloudflare URL Scanner (Behavioral & Network scanning)
    services.Configure<CloudflareUrlScannerOptions>(config.GetSection("CloudflareUrlScanner"));
    services.AddHttpClient<ICloudflareUrlScannerService, CloudflareUrlScannerService>();

    // Email Validation
    services.Configure<AbstractEmailValidationOptions>(config.GetSection("AbstractEmailValidation"));
    services.AddHttpClient<IEmailValidationService, AbstractEmailValidationService>();

    // Redirect Resolution
    services.AddHttpClient<IUrlRedirectResolver, UrlRedirectResolver>()
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            MaxAutomaticRedirections = 1
        });

    logger.LogInformation("{Project} services registered", "Infrastructure");


    return services;
  }
}
