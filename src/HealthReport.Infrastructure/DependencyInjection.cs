using HealthReport.Application.Interfaces;
using HealthReport.Infrastructure.Repositories;
using HealthReport.Infrastructure.TempFileStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthReport.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext (PostgreSQL)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContextPool<HealthReportDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        // Register generic repository implementation
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Temp file storage configuration and registration
        services.Configure<FileSystemTempFileStorageOptions>(configuration.GetSection("FileSystemTempFileStorage"));
        services.AddSingleton<ITempFileStorage, FileSystemTempFileStorage>();
        
        return services;
    }

}