namespace HealthReport.Identity.Contracts.UpdateUser;
/// <summary>
/// Result of a user update operation.
/// Contains information whether the operation succeeded and an array of error messages originating from Identity (for example from UserManager/RoleManager).
/// </summary>
/// <param name="Succeeded">True when all operations (user update and role synchronization) succeeded.</param>
/// <param name="Errors">Array of error messages. An empty array means no errors.</param>
public record UpdateUserResultDto(bool Succeeded, string[] Errors);