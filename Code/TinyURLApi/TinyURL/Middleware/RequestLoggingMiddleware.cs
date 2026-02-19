using System.Text;
using System.Text.Json;
using TinyURL.Data;
using TinyURL.Models;

namespace TinyURL.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext db)
        {
            // ⭐ Allow request body to be read multiple times
            context.Request.EnableBuffering();

            // ✅ Capture Headers
            var headersDict = context.Request.Headers
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            // ✅ Capture ShortCode (if exists)
            var shortCode = context.Request.RouteValues["shortCode"]?.ToString();

            // ✅ Capture Raw Body
            string body = "";
            using (var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // ✅ Capture RouteValues JSON
            var routeValuesDict = context.Request.RouteValues
                .ToDictionary(r => r.Key, r => r.Value?.ToString());

            var routeValuesJson = JsonSerializer.Serialize(routeValuesDict);

            // ✅ Capture QueryString
            var queryString = context.Request.QueryString.ToString();

            // ✅ Create Log Object
            var log = new RequestLog
            {
                Endpoint = context.Request.Path,
                Method = context.Request.Method,
                Headers = JsonSerializer.Serialize(headersDict),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                ShortCode = shortCode,

                // ⭐ NEW RAW DATA
                RawBody = body,
                QueryString = queryString,
                RouteValues = routeValuesJson,

                CreatedAt = DateTime.UtcNow
            };

            // Execute next middleware / controller
            await _next(context);

            // Capture response status
            log.StatusCode = context.Response.StatusCode;

            // Save log
            db.RequestLogs.Add(log);
            await db.SaveChangesAsync();
        }
    }
}
