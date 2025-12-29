using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HealthReport.Web.Extensions
{
    /// <summary>
    /// Kolekcja rozszerzeń rejestrujących middleware specyficzne dla aplikacji.
    /// Tutaj grupujemy różne middleware (np. API error handling, request logging itp.).
    /// </summary>
    public static class Middlewares
    {
        /// <summary>
        /// Rejestruje middleware specyficzne dla HealthReport Web.
        /// Obecnie konwertuje 404 dla ścieżek zaczynających się od `/api` na JSON.
        /// W przyszłości dodawaj tu kolejne middleware.
        /// </summary>
        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted && context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.ContentType = "application/json";
                    var payload = JsonSerializer.Serialize(new { error = "Not found" });
                    await context.Response.WriteAsync(payload);
                }
            });

            return app;
        }
    }
}
