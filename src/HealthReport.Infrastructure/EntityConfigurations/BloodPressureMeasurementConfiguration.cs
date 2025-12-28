using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthReport.Domain.Entities;

namespace HealthReport.Infrastructure.EntityConfigurations
{
    public class BloodPressureMeasurementConfiguration : IEntityTypeConfiguration<BloodPressureMeasurement>
    {
        public void Configure(EntityTypeBuilder<BloodPressureMeasurement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MeasuredDate).IsRequired();
            builder.Property(x => x.MeasuredTime).IsRequired();
            builder.Property(x => x.Systolic).IsRequired();
            builder.Property(x => x.Diastolic).IsRequired();
            builder.Property(x => x.Pulse);
            builder.Property(x => x.Note).HasMaxLength(1000);
        }
    }
}
