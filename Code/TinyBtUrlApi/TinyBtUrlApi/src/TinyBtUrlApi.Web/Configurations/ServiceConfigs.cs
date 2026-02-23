using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure;

namespace TinyBtUrlApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger);

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }


}
