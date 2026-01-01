namespace HealthReport.Identity.Contracts;

/// <summary>
/// Data transfer object for user registration details.
/// </summary>
/// <param name="Email">The email address of the user to be registered.</param>
/// <param name="Password">The password of the user to be registered.</param>
public record RegisterUserDto(string Email, string Password);