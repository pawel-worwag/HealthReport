using HealthReport.Identity.Contracts.UsersList;

namespace HealthReport.Identity.Application;

public interface IUsersListHandler
{
    Task<ICollection<UserDto>> ListAsync(CancellationToken cancellationToken = default);
}