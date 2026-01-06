using HealthReport.Application.Handlers.Imports;
using HealthReport.Application.Handlers.Reports.Simple;
using HealthReport.Application.Handlers.Reports.SimpleAvg;
using HealthReport.Application.Handlers.Reports.SimpleSvg2;
using HealthReport.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthReport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Import handlers
        services.AddScoped<BloodPressureIHealthImportHandler>();
        services.AddScoped<BloodGlucoseContourImportHandler>();
        services.AddScoped<WeightGarminImportHandler>();
        services.AddTransient<ImportHandlerFactory>();
        // Reports handlers
        services.AddScoped<ISimpleReportHandler, SimpleReportHandler>();
        services.AddScoped<ISimpleAvgReportHandler, SimpleAvgReportHandler>();
        return services;
    }
}