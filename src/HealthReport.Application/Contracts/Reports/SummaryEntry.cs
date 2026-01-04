using System.Text.Json.Serialization;

namespace HealthReport.Application.Contracts.Reports;

public record SummaryEntry
{
    [JsonPropertyName("diastolic-summary")] public SummaryEntryValue DiastolicSummary { get; init; } = new();
    [JsonPropertyName("systolic-summary")] public SummaryEntryValue SystolicSummary { get; init; } = new();
    [JsonPropertyName("pulse-summary")] public SummaryEntryValue PulseSummary { get; init; } = new();
    [JsonPropertyName("glucose-summary")] public SummaryEntryValue GlucoseSummary { get; init; } = new();
    [JsonPropertyName("weight-summary")] public SummaryEntryValue WeightSummary { get; init; } = new();
    [JsonPropertyName("bmi-summary")] public SummaryEntryValue BmiSummary { get; init; } = new();
}