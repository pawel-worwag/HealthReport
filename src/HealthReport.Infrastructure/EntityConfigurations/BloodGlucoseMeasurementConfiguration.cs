using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthReport.Domain.Entities;

namespace HealthReport.Infrastructure.EntityConfigurations
{
    public class BloodGlucoseMeasurementConfiguration : IEntityTypeConfiguration<BloodGlucoseMeasurement>
    {
        public void Configure(EntityTypeBuilder<BloodGlucoseMeasurement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MeasuredDate).IsRequired();
            builder.Property(x => x.MeasuredTime).IsRequired();
            builder.Property(x => x.BGValue).IsRequired();
            builder.Property(x => x.Meal).IsRequired();
            builder.Property(x => x.Note).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
