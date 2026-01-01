namespace HealthReport.Identity.Contracts.UsersList;

public record UserDto(Guid Id, string Email, string UserName, int AccessFailedCount, bool LockoutEnabled, System.DateTimeOffset? LockoutEnd);