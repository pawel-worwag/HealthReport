using HealthReport.Identity.Contracts;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application;

/// <summary>
/// Handles the login process for users.
/// </summary>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
public class LoginUserHandler(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager) : ILoginUserHandler
{
    /// <inheritdoc />
    public async Task<LoginUserResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return new LoginUserResultDto(false, null, [ "Bad user or invalid credentials" ]);
        }
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return new LoginUserResultDto(false, null, ["Bad user or invalid credentials"]);
        }

        var valid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
        {
            return new LoginUserResultDto(false, null, ["Bad user or invalid credentials"]);
        }
        await signInManager.SignInAsync(user, isPersistent: false);
        return new LoginUserResultDto(true, user.Id, []);
    }
}
