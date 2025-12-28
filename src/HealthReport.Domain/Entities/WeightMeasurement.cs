using System;

namespace HealthReport.Domain.Entities
{

    public enum MeasurementSource
    {
        Unknown = 0,
        GarminConnect = 1,
        Manual = 2,
        Professional = 3
    }
    
    /// <summary>
    /// Represents a body weight measurement (e.g. exported from Garmin Connect).
    /// Date and time are stored separately using DateOnly and TimeOnly.
    /// </summary>
    public class WeightMeasurement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PatientId { get; set; }

        // Date and time stored separately
        public DateOnly MeasuredDate { get; set; }
        public TimeOnly MeasuredTime { get; set; }

        /// <summary>
        /// Weight in kilograms.
        /// </summary>
        public decimal WeightKg { get; set; }

        /// <summary>
        /// Change since previous measurement in kilograms (can be negative).
        /// Nullable if not available.
        /// </summary>
        public decimal? WeightChangeKg { get; set; }

        /// <summary>
        /// Body mass index.
        /// Nullable if not provided by the source.
        /// </summary>
        public decimal? BMI { get; set; }

        /// <summary>
        /// Body fat percentage (0-100).
        /// </summary>
        public decimal? BodyFatPercentage { get; set; }

        /// <summary>
        /// Skeletal muscle mass in kilograms (if reported).
        /// </summary>
        public decimal? SkeletalMuscleMassKg { get; set; }

        /// <summary>
        /// Body water percentage (0-100).
        /// </summary>
        public decimal? BodyWaterPercentage { get; set; }

        /// <summary>
        /// Source of the measurement (e.g. GarminConnect, Manual entry, Professional measurement).
        /// </summary>
        public MeasurementSource Source { get; set; } = MeasurementSource.Unknown;

        /// <summary>
        /// Optional textual details about the source (device id, notes from professional, etc.).
        /// </summary>
        public string? SourceDetails { get; set; }

        /// <summary>
        /// Optional note or comment attached to the measurement.
        /// </summary>
        public string? Note { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
