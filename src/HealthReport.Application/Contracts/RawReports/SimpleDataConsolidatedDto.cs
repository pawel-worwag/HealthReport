using HealthReport.Domain.Entities;

namespace HealthReport.Application.Contracts.RawReports;

public record SimpleDataConsolidatedDto
{
    public required DateOnly Date { get; init; }
    public required ICollection<BloodPressureDto> BloodPressure { get; init; }
    public required ICollection<BloodGlucoseDto> BloodGlucose { get; init; }
    public required ICollection<WeightDto> Weight { get; init; }
 
}

public record BloodPressureDto
{
    public TimeOnly? MeasuredTime { get; init; }
    public decimal? Systolic { get; init; }
    public decimal? Diastolic { get; init; }
    public int? Pulse { get; init; }
}

public record BloodGlucoseDto
{
    public TimeOnly? MeasuredTime { get; init; }
    public int? Glucose { get; init; }
    public MealMarker? Meal { get; init; }
}

public record WeightDto
{
    public TimeOnly? MeasuredTime { get; init; }
    public decimal? Weight { get; init; }
    public decimal? WeightChange { get; init; }
    public decimal? Bmi { get; init; }
    public decimal? BodyFatPercentage { get; init; }
    public decimal? SkeletalMuscleMass { get; init; }
    public decimal? BodyWaterPercentage { get; init; }
}