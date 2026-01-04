using System.Text.Json.Serialization;

namespace HealthReport.Application.FileParsers;

public class CsvParsingResult<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public IEnumerable<ParsingError> Errors { get; set; } = Enumerable.Empty<ParsingError>();
    
}

public record ParsingError
{
    
    [JsonPropertyName("line")]
    public int Line { get; init; }
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}