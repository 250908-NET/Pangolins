using System.Diagnostics;

namespace Pangolivia.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var method = context.Request.Method;
            var path = context.Request.Path.HasValue ? context.Request.Path.Value : "/";
            var elapsed = sw.ElapsedMilliseconds;

            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "Method: ({Method}), Path: ({Path}), Response Time: ({Elapsed} ms)",
                    method,
                    path,
                    elapsed
                );
            }
            else
            {
                _logger.LogInformation(
                    "Method: ({Method}), Path: ({Path}), Response Time: ({Elapsed} ms)",
                    method,
                    path,
                    elapsed
                );
            }
        }
    }
}
