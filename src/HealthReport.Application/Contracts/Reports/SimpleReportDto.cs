using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace HealthReport.Application.Contracts.Reports
{
    public record SimpleReportDto
    {
        [JsonPropertyName("year")] public int Year { get; init; }
        [JsonPropertyName("month")] public int Month { get; init; }

        [JsonPropertyName("summary")] public SummaryEntry Summary { get; init; } = new();
        [JsonPropertyName("details")] public IReadOnlyList<DetailEntry> Details { get; init; } = Array.Empty<DetailEntry>();
    }
    
    
    public record DetailEntry
    {
        [JsonPropertyName("date")] public DateOnly Date { get; init; }
        [JsonPropertyName("blood-pressure")] public IReadOnlyList<BloodPressureEntry> BloodPressure { get; init; } = Array.Empty<BloodPressureEntry>();
        [JsonPropertyName("glucose")] public IReadOnlyList<GlucoseEntry> Glucose { get; init; } = Array.Empty<GlucoseEntry>();
        [JsonPropertyName("weight")] public IReadOnlyList<WeightEntry> Weight { get; init; } = Array.Empty<WeightEntry>();
    }

    public record BloodPressureEntry
    {
        [JsonPropertyName("time")] public TimeOnly Time { get; init; }
        [JsonPropertyName("systolic")] public int Systolic { get; init; }
        [JsonPropertyName("diastolic")] public int Diastolic { get; init; }
        [JsonPropertyName("pulse")] public int? Pulse { get; init; }
        [JsonPropertyName("note")] public string? Note { get; init; }
    }

    public record GlucoseEntry
    {
        [JsonPropertyName("time")] public TimeOnly Time { get; init; }
        [JsonPropertyName("value")] public int Value { get; init; }
        [JsonPropertyName("meal")] public int Meal { get; init; }
        [JsonPropertyName("note")] public string? Note { get; init; }
    }

    public record WeightEntry
    {
        [JsonPropertyName("time")] public TimeOnly Time { get; init; }
        [JsonPropertyName("weight-kg")] public decimal WeightKg { get; init; }
        [JsonPropertyName("bmi")] public decimal? Bmi { get; init; }
    }
}
