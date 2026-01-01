namespace HealthReport.Identity.Contracts;

/// <summary>
/// Data transfer object representing the result of a user login attempt.
/// </summary>
/// <param name="Succeeded">A value indicating whether the login attempt was successful.</param>
/// <param name="UserId">The unique identifier of the logged-in user, if the attempt was successful.</param>
/// <param name="Errors">A collection of error messages if the login attempt failed.</param>
public record LoginUserResultDto(bool Succeeded, Guid? UserId, string[] Errors);
