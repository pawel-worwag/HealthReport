using HealthReport.Application.Handlers.Imports;
using HealthReport.Application.Handlers.Reports.Simple;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthReport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Import handlers
        services.AddScoped<IBloodPressureImportHandler, BloodPressureImportHandler>();
        services.AddScoped<IBloodGlucoseImportHandler, BloodGlucoseImportHandler>();
        services.AddScoped<IWeightImportHandler, WeightImportHandler>();
        services.AddTransient<ImportHandlerFactory>();
        // Reports handlers
        services.AddScoped<ISimpleReportHandler, SimpleReportHandler>();
        return services;
    }
}