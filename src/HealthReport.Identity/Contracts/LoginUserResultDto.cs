namespace HealthReport.Identity.Contracts;

public record LoginUserResultDto(bool Succeeded, Guid? UserId, string[] Errors);
