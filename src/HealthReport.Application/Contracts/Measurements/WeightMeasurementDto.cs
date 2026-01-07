using System;

namespace HealthReport.Application.Contracts.Measurements
{
    /// <summary>
    /// DTO for weight measurements exposed by application handlers.
    /// Does not include PatientId.
    /// </summary>
    public record WeightMeasurementDto
    {
        public required Guid Id { get; init; }
        public required DateOnly MeasuredDate { get; init; }
        public required TimeOnly MeasuredTime { get; init; }
        public decimal WeightKg { get; init; }
        public decimal? WeightChangeKg { get; init; }
        public decimal? BMI { get; init; }
        public decimal? BodyFatPercentage { get; init; }
        public decimal? SkeletalMuscleMassKg { get; init; }
        public decimal? BodyWaterPercentage { get; init; }
        public HealthReport.Domain.Entities.MeasurementSource Source { get; init; }
        public string? SourceDetails { get; init; }
        public string? Note { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
