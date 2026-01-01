using HealthReport.Identity.Contracts.UsersList;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthReport.Identity.Application;

public class UsersListHandler(UserManager<ApplicationUser> userManager) : IUsersListHandler
{
    public async Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await userManager.Users
            .Select(u => new UserDto(u.Id, u.Email ?? string.Empty, u.UserName ?? string.Empty)).OrderBy(u => u.Email)
            .ToArrayAsync(cancellationToken);
    }
}