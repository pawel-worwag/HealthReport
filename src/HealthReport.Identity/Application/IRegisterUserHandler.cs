using HealthReport.Identity.Contracts;

namespace HealthReport.Identity.Application;

/// <summary>
/// Defines a handler for registering new users.
/// </summary>
public interface IRegisterUserHandler
{
    /// <summary>
    /// Registers a new user based on the provided registration data.
    /// </summary>
    /// <param name="dto">The data transfer object containing registration details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the registration result.</returns>
    Task<RegisterUserResultDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
}