using System.Linq;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace HealthReport.Web.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services)
    {
        // Register application health checks (DB, external services, etc.)
        services.AddHealthChecks()
            .AddCheck<HealthReport.Infrastructure.HealthChecks.DbContextHealthCheck>("dbcontext", tags: new[] { "critical" });

        return services;
    }

    public static WebApplication MapHealthChecksEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("critical"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var payload = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), description = e.Value.Description })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(payload));
            }
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            // Liveness doesn't run readiness checks; keep it lightweight
            Predicate = _ => false
        });

        return app;
    }
}
