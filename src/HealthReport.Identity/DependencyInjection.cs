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
        
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<HealthReportIdentityDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

}