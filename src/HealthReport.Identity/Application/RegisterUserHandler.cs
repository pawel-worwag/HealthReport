using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Identity.Contracts;
using HealthReport.Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace HealthReport.Identity.Application
{
    public class RegisterUserHandler(UserManager<ApplicationUser> userManager) : IRegisterUserHandler
    {
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
