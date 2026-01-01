using HealthReport.Identity.Contracts.UserDetails;

namespace HealthReport.Identity.Application;

/// <summary>
/// Handler for retrieving user details including assigned roles.
/// </summary>
public interface IUserDetailsHandler
{
    /// <summary>
    /// Gets user details by id or returns null when not found.
    /// </summary>
    Task<UserDetailsDto?> GetByIdAsync(System.Guid id, CancellationToken cancellationToken = default);
}
