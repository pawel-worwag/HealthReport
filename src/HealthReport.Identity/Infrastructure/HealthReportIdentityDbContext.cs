using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Identity.Infrastructure;

public class HealthReportIdentityDbContext(DbContextOptions<HealthReportIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("identity");
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(HealthReportIdentityDbContext).Assembly);
    }
}