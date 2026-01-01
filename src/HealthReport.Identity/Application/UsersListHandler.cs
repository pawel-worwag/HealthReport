using HealthReport.Identity.Contracts.UsersList;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Identity.Application;

/// <summary>
/// Handles the retrieval of the users list.
/// </summary>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
public class UsersListHandler(UserManager<ApplicationUser> userManager) : IUsersListHandler
{
    /// <inheritdoc />
        public async Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        // Order by database field first, then project to DTO so EF Core can translate the query.
        return await userManager.Users
            .OrderBy(u => u.Email)
            .Select(u => new UserDto(
                u.Id,
                u.Email ?? string.Empty,
                u.UserName ?? string.Empty,
                u.AccessFailedCount,
                u.LockoutEnabled,
                u.LockoutEnd
            ))
            .ToArrayAsync(cancellationToken);
    }
}