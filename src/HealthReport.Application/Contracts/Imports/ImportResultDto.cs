using System.Text.Json.Serialization;
using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Contracts.Imports;

public record ImportResultDto
{
    [JsonPropertyName("imported")]
    public required int Imported { get; init; }
    [JsonPropertyName("errors")]
    public IEnumerable<ParseError> Errors { get; init; } = Enumerable.Empty<ParseError>();
}