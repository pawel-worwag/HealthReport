using HealthReport.Identity.Contracts;

namespace HealthReport.Identity.Application;

public interface ILoginUserHandler
{
    Task<LoginUserResultDto> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
}
