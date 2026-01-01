using HealthReport.Identity.Contracts;

namespace HealthReport.Identity.Application;

/// <summary>
/// Defines a handler for logging in users.
/// </summary>
public interface ILoginUserHandler
{
    /// <summary>
    /// Logs in a user based on the provided credentials.
    /// </summary>
    /// <param name="dto">The data transfer object containing login credentials.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the login result.</returns>
    Task<LoginUserResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
}
