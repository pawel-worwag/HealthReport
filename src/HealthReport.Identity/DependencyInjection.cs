using HealthReport.Identity.Domain;
using HealthReport.Identity.Extensions;
using HealthReport.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

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
        services.AddScoped<Application.IRegisterUserHandler, Application.RegisterUserHandler>();
        services.AddScoped<Application.ILoginUserHandler, Application.LoginUserHandler>();
        services.AddScoped<Application.IUsersListHandler, Application.UsersListHandler>();
        services.AddScoped<Application.IRolesListHandler, Application.RolesListHandler>();

        return services;
    }

    public static WebApplication MapIdentityEndpoints(this WebApplication app)
    {
        app.MapEndpoints();
        return app;
    }

}