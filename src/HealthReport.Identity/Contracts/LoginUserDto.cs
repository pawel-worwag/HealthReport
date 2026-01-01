namespace HealthReport.Identity.Contracts;

/// <summary>
/// Data transfer object for user login credentials.
/// </summary>
/// <param name="Email">The email address of the user attempting to log in.</param>
/// <param name="Password">The password of the user attempting to log in.</param>
public record LoginUserDto(string Email, string Password);
