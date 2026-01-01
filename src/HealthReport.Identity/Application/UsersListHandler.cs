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
        return await userManager.Users
            .Select(u => new UserDto(u.Id, u.Email ?? string.Empty, u.UserName ?? string.Empty)).OrderBy(u => u.Email)
            .ToArrayAsync(cancellationToken);
    }
}