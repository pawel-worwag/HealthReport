namespace HealthReport.Identity.Contracts.UsersList;

/// <summary>
/// Data transfer object representing a user in the user list.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="UserName">The name of the user.</param>
/// <param name="AccessFailedCount">The number of failed login attempts for the user.</param>
/// <param name="LockoutEnabled">A value indicating whether the user can be locked out.</param>
/// <param name="LockoutEnd">The date and time when the user lockout ends, if applicable.</param>
public record UserDto(Guid Id, string Email, string UserName, int AccessFailedCount, bool LockoutEnabled, System.DateTimeOffset? LockoutEnd);