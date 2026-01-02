namespace HealthReport.Identity.Contracts.UpdateUser;
/// <summary>
/// DTO containing data for updating a user.
/// Used by a handler that updates basic user fields and their assigned roles.
/// </summary>
/// <param name="Id">User identifier (required).</param>
/// <param name="Roles">Collection of role names the user should have after the update. The provided list replaces existing roles (synchronization).</param>
public record UpdateUserDto(System.Guid Id, ICollection<string> Roles);