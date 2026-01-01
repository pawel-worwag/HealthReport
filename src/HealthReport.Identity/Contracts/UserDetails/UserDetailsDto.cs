namespace HealthReport.Identity.Contracts.UserDetails;

/// <summary>
/// Data transfer object containing user details and assigned roles.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Email">User email.</param>
/// <param name="UserName">User name.</param>
/// <param name="Roles">Assigned roles.</param>
public record UserDetailsDto(
	System.Guid Id,
	string Email,
	string UserName,
	string NormalizedUserName,
	string NormalizedEmail,
	int AccessFailedCount,
	bool LockoutEnabled,
	System.DateTimeOffset? LockoutEnd,
	System.Collections.Generic.ICollection<string> Roles
);
