using System.Collections.Generic;

namespace HealthReport.Web.Contracts;

public sealed record HealthCheckEntryDto(string Name, string Status, string? Description);

public sealed record HealthResponseDto(string Status, List<HealthCheckEntryDto> Checks);
