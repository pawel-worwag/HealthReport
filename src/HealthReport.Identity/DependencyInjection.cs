using HealthReport.Identity.Domain;
using HealthReport.Identity.Extensions;
using HealthReport.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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
        services.AddScoped<HealthReport.Identity.Application.ILoginUserHandler, HealthReport.Identity.Application.LoginUserHandler>();
        return services;
    }

    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapEndpoints();
        return app;
    }

}