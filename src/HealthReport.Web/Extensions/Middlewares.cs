using HealthReport.Application.Errors;
using Microsoft.Extensions.Logging.Abstractions;

namespace HealthReport.Web.Extensions
{
    public static class Middlewares
    {
        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            var loggerFactory = app.Services.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("HealthReport.Web.Middlewares") ?? NullLogger.Instance;

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted && context.Request.Path.StartsWithSegments("/api"))
                {
                    var apiError = new ApiError()
                    {
                        Message = "Not found"
                    };
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(apiError);
                }
            });

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    // Log the exception for API requests and return JSON error
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        logger.LogError(ex, "Unhandled exception while processing API request {Path}", context.Request.Path);
                        var apiError = new ApiError()
                        {
                            Message = ex.Message
                        };
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(apiError);
                        return;
                    }

                    // Non-API: rethrow to let upstream exception handler deal with it
                    throw;
                }
            });
            
            return app;
        }
    }
}
