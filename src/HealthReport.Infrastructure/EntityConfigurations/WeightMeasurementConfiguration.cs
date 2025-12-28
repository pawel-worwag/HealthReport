using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HealthReport.Domain.Entities;

namespace HealthReport.Infrastructure.EntityConfigurations
{
    public class WeightMeasurementConfiguration : IEntityTypeConfiguration<WeightMeasurement>
    {
        public void Configure(EntityTypeBuilder<WeightMeasurement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MeasuredDate).IsRequired();
            builder.Property(x => x.MeasuredTime).IsRequired();
            builder.Property(x => x.WeightKg).IsRequired().HasPrecision(8,2);
            builder.Property(x => x.WeightChangeKg).HasPrecision(8,2);
            builder.Property(x => x.BMI).HasPrecision(5,2);
            builder.Property(x => x.BodyFatPercentage).HasPrecision(5,2);
            builder.Property(x => x.SkeletalMuscleMassKg).HasPrecision(8,2);
            builder.Property(x => x.BodyWaterPercentage).HasPrecision(5,2);
            builder.Property(x => x.Source).IsRequired();
            builder.Property(x => x.SourceDetails).HasMaxLength(500);
            builder.Property(x => x.Note).HasMaxLength(1000);
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
