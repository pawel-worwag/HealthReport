using HealthReport.Identity.Domain;
using HealthReport.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthReport.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<HealthReportIdentityDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), o => o.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));
        
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<HealthReportIdentityDbContext>();

        // Application handlers
        services.AddScoped<HealthReport.Identity.Application.IRegisterUserHandler, HealthReport.Identity.Application.RegisterUserHandler>();
        return services;
    }

}