using System.Text.Json.Serialization;

namespace HealthReport.Application.Contracts.Reports;

public class SimpleAvgReportDto
{
    [JsonPropertyName("records")] 
    public ICollection<SimpleAvgReportEntry> Records { get; init; } = new List<SimpleAvgReportEntry>();
}

public record SimpleAvgReportEntry
{
    [JsonPropertyName("year")] public int Year { get; init; }
    [JsonPropertyName("month")] public int Month { get; init; }
    [JsonPropertyName("summary")] public SummaryEntry Summary { get; init; } = new();
}
