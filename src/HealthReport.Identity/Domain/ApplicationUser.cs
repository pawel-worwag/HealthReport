namespace HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user in the application, extending the default ASP.NET Core Identity user with a <see cref="Guid"/> key.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
}