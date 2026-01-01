namespace HealthReport.Identity.Contracts;

/// <summary>
/// Data transfer object representing the result of a user registration attempt.
/// </summary>
/// <param name="Succeeded">A value indicating whether the registration attempt was successful.</param>
/// <param name="UserId">The unique identifier of the newly registered user, if the attempt was successful.</param>
/// <param name="Errors">A collection of error messages if the registration attempt failed.</param>
public record RegisterUserResultDto(bool Succeeded, Guid? UserId, string[] Errors);