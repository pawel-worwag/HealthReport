using System.Text.Json.Serialization;

namespace HealthReport.Application.FileParsers;

public class ParseResult<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public IEnumerable<ParseError> Errors { get; set; } = Enumerable.Empty<ParseError>();
    
}

public record ParseError
{
    
    [JsonPropertyName("line")]
    public int Line { get; init; }
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}