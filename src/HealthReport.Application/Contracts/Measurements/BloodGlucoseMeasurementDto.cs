using System;

namespace HealthReport.Application.Contracts.Measurements
{
    /// <summary>
    /// DTO for blood glucose measurements exposed by application handlers.
    /// Note: intentionally does not contain PatientId.
    /// </summary>
    public record BloodGlucoseMeasurementDto
    {
        public required Guid Id { get; init; }
        public required DateOnly MeasuredDate { get; init; }
        public required TimeOnly MeasuredTime { get; init; }
        public int Glucose { get; init; }
        public HealthReport.Domain.Entities.MealMarker Meal { get; init; }
        public string? Note { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
