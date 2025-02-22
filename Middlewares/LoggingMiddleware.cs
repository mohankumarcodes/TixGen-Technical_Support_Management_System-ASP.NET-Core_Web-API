using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TixGen.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;   
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation($" Request : {context.Request.Method}    {context.Request.Path}");
            _logger.LogInformation($" Middleware : {nameof(LoggingMiddleware)} ");

            await _next(context); // Call the next middleware in the pipeline

            _logger.LogInformation($" Response : {context.Response.StatusCode}");
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
