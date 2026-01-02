using HealthReport.Identity.Contracts.UpdateUser;

namespace HealthReport.Identity.Application;

/// <summary>
/// Handler for updating user data and assigned roles.
/// </summary>
public interface IUpdateUserHandler
{
    Task<UpdateUserResultDto> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken = default);
}