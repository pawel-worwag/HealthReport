using System;

namespace HealthReport.Application.Contracts.Measurements
{
    /// <summary>
    /// Data transfer object for blood pressure measurements.
    /// </summary>
    public record BloodPressureMeasurementDto
    {
        public required Guid Id { get; init; }
        public required DateOnly MeasuredDate { get; init; }
        public required TimeOnly MeasuredTime { get; init; }
        public int? WeekOfYear { get; init; }
        public int Systolic { get; init; }
        public int Diastolic { get; init; }
        public int? Pulse { get; init; }
        public string? Note { get; init; }
        public string? Source { get; init; }
    }
}
