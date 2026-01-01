using HealthReport.Identity.Contracts.UsersList;

namespace HealthReport.Identity.Application;

/// <summary>
/// Defines a handler for retrieving a list of users.
/// </summary>
public interface IUsersListHandler
{
    /// <summary>
    /// Retrieves a collection of all users.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of users.</returns>
    Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default);
}