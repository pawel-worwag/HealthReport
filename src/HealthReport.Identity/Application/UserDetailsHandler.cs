using HealthReport.Identity.Contracts.UserDetails;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application;

/// <summary>
/// Retrieves user details and assigned roles from Identity store.
/// </summary>
public class UserDetailsHandler(UserManager<ApplicationUser> userManager) : IUserDetailsHandler
{
    public async Task<UserDetailsDto?> GetByIdAsync(System.Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);

        return new UserDetailsDto(
            user.Id,
            user.Email ?? string.Empty,
            user.UserName ?? string.Empty,
            user.NormalizedUserName ?? string.Empty,
            user.NormalizedEmail ?? string.Empty,
            user.AccessFailedCount,
            user.LockoutEnabled,
            user.LockoutEnd,
            roles
        );
    }
}
