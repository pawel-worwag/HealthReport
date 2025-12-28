using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Infrastructure
{
    public class HealthReportDbContext : DbContext
    {
        public HealthReportDbContext(DbContextOptions<HealthReportDbContext> options) : base(options)
        {
        }

        public DbSet<BloodPressureMeasurement> BloodPressureMeasurements { get; set; } = null!;
        public DbSet<BloodGlucoseMeasurement> BloodGlucoseMeasurements { get; set; } = null!;
        public DbSet<WeightMeasurement> WeightMeasurements { get; set; } = null!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply IEntityTypeConfiguration<T> classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthReportDbContext).Assembly);
        }
    }
}
