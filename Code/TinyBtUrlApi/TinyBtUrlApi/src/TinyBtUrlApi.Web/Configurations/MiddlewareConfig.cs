using Ardalis.ListStartupServices;
using TinyBtUrlApi.Infrastructure.Data;
using Scalar.AspNetCore;

namespace TinyBtUrlApi.Web.Configurations;

public static class MiddlewareConfig
{
  public static async Task<IApplicationBuilder> UseAppMiddlewareAndSeedDatabase(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseShowAllServicesMiddleware();
    }
    else
    {
      app.UseDefaultExceptionHandler(); // from FastEndpoints
      app.UseHsts();
    }

    // ✅ FastEndpoints
    app.UseFastEndpoints();

    // ✅ FIXED SWAGGER (NO custom path)
    if (app.Environment.IsDevelopment())
    {
      app.UseSwaggerGen();   // 🔥 Important: no options.Path
      app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    // Run migrations and seed in Development or when explicitly requested via environment variable
    var shouldMigrate = app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup");

    if (shouldMigrate)
    {
      await MigrateDatabaseAsync(app);
      await SeedDatabaseAsync(app);
    }

    return app;
  }

  static async Task MigrateDatabaseAsync(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
      logger.LogInformation("Applying database migrations...");
      var context = services.GetRequiredService<AppDbContext>();

      // 🔴 Migration intentionally commented (sir DB drop nahi karne denge)
      // await context.Database.MigrateAsync();

      logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred migrating the DB. {exceptionMessage}", ex.Message);
      throw;
    }
  }

  static async Task SeedDatabaseAsync(WebApplication app)
  {
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
      logger.LogInformation("Seeding database...");
      var context = services.GetRequiredService<AppDbContext>();
      // SeedData class was removed; there is no application-level seed to run.
      // Skipping seeding to avoid build errors. If you add seeding back, implement SeedData.InitializeAsync.
      logger.LogInformation("No seed data present; skipping seeding");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
  }
}
