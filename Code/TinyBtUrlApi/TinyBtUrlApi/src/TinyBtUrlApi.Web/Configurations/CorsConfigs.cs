namespace TinyBtUrlApi.Web.Configurations;

public static class CorsConfigs
{
  public static IServiceCollection AddCorsConfigs(
      this IServiceCollection services)
  {
    services.AddCors(options =>
    {
      options.AddPolicy("AllowFrontend",
          policy =>
          {
            policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
          });
    });

    return services;
  }

  public static WebApplication UseCorsConfigs(this WebApplication app)
  {
    app.UseCors("AllowFrontend");
    return app;
  }
}
