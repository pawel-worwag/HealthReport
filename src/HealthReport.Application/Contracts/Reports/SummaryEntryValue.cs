using System.Text.Json.Serialization;

namespace HealthReport.Application.Contracts.Reports;

public record SummaryEntryValue
{
    [JsonPropertyName("min")] public decimal? Min { get; init; }
    [JsonPropertyName("max")] public decimal? Max { get; init; }
    [JsonPropertyName("avg")] public decimal? Avg { get; init; }
}