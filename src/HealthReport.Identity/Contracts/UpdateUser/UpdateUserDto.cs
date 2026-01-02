namespace HealthReport.Identity.Contracts.UpdateUser;
/// <summary>
/// DTO containing data for updating a user.
/// Used by a handler that updates basic user fields and their assigned roles.
/// </summary>
/// <param name="Id">User identifier (required).</param>
/// <param name="Email">New email address. Passing <c>null</c> means no change.</param>
/// <param name="UserName">New user name. Passing <c>null</c> means no change.</param>
/// <param name="Roles">Collection of role names the user should have after the update. The provided list replaces existing roles (synchronization).</param>
public record UpdateUserDto(System.Guid Id, string? Email, string? UserName, System.Collections.Generic.ICollection<string> Roles);