namespace HealthReport.Identity.Application;

public interface IRolesListHandler
{
    Task<ICollection<string>> ListAsync(CancellationToken cancellationToken = default);
}