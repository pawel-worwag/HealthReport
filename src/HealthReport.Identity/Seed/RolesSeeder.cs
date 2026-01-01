using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HealthReport.Identity.Seed;


public static class RolesSeeder
{
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        var logger = app.Logger;
        
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        
        var roles = new[] { "administrator", "user" };

        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role)) continue;
            
            var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            if (result.Succeeded)
            {
                logger?.LogInformation("Created role '{role}'", role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger?.LogWarning("Failed to create role '{role}': {errors}", role, errors);
            }
        }
    }
}
