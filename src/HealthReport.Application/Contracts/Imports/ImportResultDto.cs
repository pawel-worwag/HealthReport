using System.Text.Json.Serialization;
using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Contracts.Imports;

public record ImportResultDto
{
    [JsonPropertyName("imported")]
    public required int Imported { get; init; }
    [JsonPropertyName("duplicates")]
    public required int Duplicates { get; init; }
    [JsonPropertyName("errors")]
    public IEnumerable<ParsingError> Errors { get; init; } = Enumerable.Empty<ParsingError>();
}