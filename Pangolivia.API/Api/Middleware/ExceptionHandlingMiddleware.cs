using System.Net;
using System.Text.Json;

namespace Pangolivia.API.Api.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Minimal JSON problem response
                var problem = new
                {
                    title = "An unhandled error occurred.",
                    status = (int)HttpStatusCode.InternalServerError,
                    traceId = context.TraceIdentifier
                };

                // Log the exception (Serilog will pick this up)
                // Avoid leaking sensitive info in the response
                Console.Error.WriteLine(ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}
