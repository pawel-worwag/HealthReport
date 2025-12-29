using System.Text.Json.Serialization;

namespace HealthReport.Application.Errors;

public record ApiError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}