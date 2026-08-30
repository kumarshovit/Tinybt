using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TinyBtUrlApi.Web.Configurations;

public static class RateLimitConfigs
{
  /// <summary>
  /// Policy name applied to API key management endpoints only.
  /// Not applied globally — no other endpoints are rate-limited.
  /// </summary>
  public const string ApiKeyManagementPolicy = "ApiKeyManagementRateLimit";

  /// <summary>
  /// Policy name applied to Developer API URL creation endpoint (/api/v1/links).
  /// Separated from key management because URL creation is a different workload.
  /// </summary>
  public const string DeveloperApiUrlCreationPolicy = "DeveloperApiUrlCreationRateLimit";


  public static IServiceCollection AddDeveloperApiRateLimiting(
      this IServiceCollection services,
      IConfiguration configuration)
  {
    var section = configuration.GetSection("DeveloperApi:RateLimiting");

    var permitLimit = section.GetValue<int?>("PermitLimit");
    var windowInSeconds = section.GetValue<int?>("WindowInSeconds");

    if (permitLimit is null or <= 0)
    {
      throw new InvalidOperationException(
        "Rate limiting configuration error: 'DeveloperApi:RateLimiting:PermitLimit' " +
        "is missing or invalid. Must be a positive integer.");
    }

    if (windowInSeconds is null or <= 0)
    {
      throw new InvalidOperationException(
        "Rate limiting configuration error: 'DeveloperApi:RateLimiting:WindowInSeconds' " +
        "is missing or invalid. Must be a positive integer.");
    }

    services.AddRateLimiter(options =>
    {
      // Partition per authenticated UserId (from API key claims).
      // UserId is safe to read here because UseRateLimiter() is placed
      // after UseAuthentication() + UseAuthorization() in the pipeline.
      options.AddPolicy(ApiKeyManagementPolicy, httpContext =>
      {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Partition by authenticated UserId. Fall back to remote IP for
        // unauthenticated requests (which will be rejected by auth middleware
        // before reaching the endpoint handler anyway).
        var partitionKey = userId is not null
          ? $"apikey_user_{userId}"
          : $"apikey_anon_{httpContext.Connection.RemoteIpAddress}";

        return RateLimitPartition.GetFixedWindowLimiter(
          partitionKey,
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = permitLimit.Value,
            Window = TimeSpan.FromSeconds(windowInSeconds.Value),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
          });
      });

      // Developer API URL Creation Policy
      var urlCreationSection = configuration.GetSection("DeveloperApi:UrlCreationRateLimiting");
      var urlCreationPermitLimit = urlCreationSection.GetValue<int?>("PermitLimit");
      var urlCreationWindowInSeconds = urlCreationSection.GetValue<int?>("WindowInSeconds");

      if (urlCreationPermitLimit is null or <= 0)
      {
        throw new InvalidOperationException(
          "Rate limiting configuration error: 'DeveloperApi:UrlCreationRateLimiting:PermitLimit' is missing or invalid.");
      }

      if (urlCreationWindowInSeconds is null or <= 0)
      {
        throw new InvalidOperationException(
          "Rate limiting configuration error: 'DeveloperApi:UrlCreationRateLimiting:WindowInSeconds' is missing or invalid.");
      }

      options.AddPolicy(DeveloperApiUrlCreationPolicy, httpContext =>
      {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var partitionKey = userId is not null
          ? $"linkapi_user_{userId}"
          : $"linkapi_anon_{httpContext.Connection.RemoteIpAddress}";

        return RateLimitPartition.GetFixedWindowLimiter(
          partitionKey,
          _ => new FixedWindowRateLimiterOptions
          {
            PermitLimit = urlCreationPermitLimit.Value,
            Window = TimeSpan.FromSeconds(urlCreationWindowInSeconds.Value),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
          });
      });

      options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

      options.OnRejected = async (context, cancellationToken) =>
      {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)Math.Max(1, Math.Ceiling(retryAfter.TotalSeconds))).ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
          new { message = "Rate limit exceeded. Please slow down your requests." },
          cancellationToken);
      };
    });

    return services;
  }
}
