namespace HealthReport.Application.Services;

public class ParseResult<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public IEnumerable<ParseError> Errors { get; set; } = Enumerable.Empty<ParseError>();
    
}

public record ParseError
{
    public int Line { get; init; }
    public string Message { get; init; } = string.Empty;
}