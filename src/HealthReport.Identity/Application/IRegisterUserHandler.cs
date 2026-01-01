using HealthReport.Identity.Contracts;

namespace HealthReport.Identity.Application;

public interface IRegisterUserHandler
{
    Task<RegisterUserResultDto> RegisterAsync(RegisterUserDto dto, CancellationToken cancellationToken = default);
}