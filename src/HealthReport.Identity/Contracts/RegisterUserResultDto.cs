namespace HealthReport.Identity.Contracts;

public record RegisterUserResultDto(bool Succeeded, Guid? UserId, string[] Errors);