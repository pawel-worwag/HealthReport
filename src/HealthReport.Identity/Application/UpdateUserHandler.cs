using HealthReport.Identity.Contracts.UpdateUser;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application;


/// <summary>
/// Handler that updates basic user data and the user's assigned roles.
/// Behavior:
/// - verifies the user exists by identifier,
/// - validates that provided roles exist in the system (uses <c>RoleManager</c>),
/// - updates basic user fields (e.g. email, user name) via <c>UserManager</c>,
/// - synchronizes assigned roles (removes and adds roles so the final set matches the supplied list),
/// - returns <see cref="HealthReport.Identity.Contracts.UpdateUser.UpdateUserResultDto"/> containing success information and any error messages produced by Identity.
/// </summary>
public class UpdateUserHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager) : IUpdateUserHandler
{
    /// <summary>
    /// Updates the user according to the data in <paramref name="dto"/>.
    /// The <c>Email</c> and <c>UserName</c> fields are optional — passing <c>null</c> means no change.
    /// The <c>Roles</c> collection is treated as the target set of roles and will be synchronized with the user's current assignments.
    /// The method returns any error messages (for example when roles do not exist or Identity operations fail).
    /// </summary>
    public async Task<UpdateUserResultDto> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Id == Guid.Empty)
        {
            return new UpdateUserResultDto(false, ["Bad id"]);
        }
        var user = await userManager.FindByIdAsync(dto.Id.ToString());
        if (user is null)
        {
            return new UpdateUserResultDto(false, ["User not found"]);
        }
        var errors = new List<string>();
        
        foreach (var role in dto.Roles ?? Array.Empty<string>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                errors.Add($"Role '{role}' does not exist");
            }
        }
        if (errors.Count != 0)
        {
            return new UpdateUserResultDto(false, errors.ToArray());
        }
        
        var currentRoles = (await userManager.GetRolesAsync(user)).ToArray();
        var newRoles = dto.Roles ?? Array.Empty<string>();
        
        var toAdd = newRoles.Except(currentRoles).ToArray();
        var toRemove = currentRoles.Except(newRoles).ToArray();
        
        if (toRemove.Length != 0)
        {
            var rem = await userManager.RemoveFromRolesAsync(user, toRemove);
            if (!rem.Succeeded) errors.AddRange(rem.Errors.Select(e => e.Description));
        }

        if (toAdd.Length != 0)
        {
            var add = await userManager.AddToRolesAsync(user, toAdd);
            if (!add.Succeeded) errors.AddRange(add.Errors.Select(e => e.Description));
        }

        return new UpdateUserResultDto((errors.Count==0),errors.ToArray());
    }
}