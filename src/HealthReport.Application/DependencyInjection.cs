using HealthReport.Application.Handlers.Imports;
using HealthReport.Application.Handlers.Reports;
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
        // Reports handlers
        services.AddScoped<IMonthlyReportHandler, MonthlyReportHandler>();
        return services;
    }
}