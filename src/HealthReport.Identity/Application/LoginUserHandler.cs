using HealthReport.Identity.Contracts;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application;

public class LoginUserHandler(UserManager<ApplicationUser> userManager) : ILoginUserHandler
{
    public async Task<LoginUserResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return new LoginUserResultDto(false, null, new[] { "Invalid credentials" });
        }

        var valid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
        {
            return new LoginUserResultDto(false, null, new[] { "Invalid credentials" });
        }

        return new LoginUserResultDto(true, user.Id, Array.Empty<string>());
    }
}
