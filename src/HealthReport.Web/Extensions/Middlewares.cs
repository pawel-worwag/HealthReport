using System.Text.Json;
using HealthReport.Application.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HealthReport.Web.Extensions
{
    public static class Middlewares
    {
        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
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
                    if(!context.Request.Path.StartsWithSegments("/api"))
                    {
                        throw;
                    }
                    var apiError = new ApiError()
                    {
                        Message = ex.Message
                    };
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(apiError);
                }
            });
            
            return app;
        }
    }
}
