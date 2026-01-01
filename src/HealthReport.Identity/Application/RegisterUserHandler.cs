
using HealthReport.Identity.Contracts;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application
{
    /// <summary>
    /// Handles the registration process for new users.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity user manager.</param>
    public class RegisterUserHandler(UserManager<ApplicationUser> userManager) : IRegisterUserHandler
    {
        /// <inheritdoc />
        public async Task<RegisterUserResultDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new RegisterUserResultDto(false, null, result.Errors.Select(e => e.Description).ToArray());
            }

            return new RegisterUserResultDto(true, user.Id, Array.Empty<string>());
        }
    }
}
