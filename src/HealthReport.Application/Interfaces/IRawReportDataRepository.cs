using HealthReport.Application.Contracts.RawReports;

namespace HealthReport.Application.Interfaces;

public interface IRawReportDataRepository
{
    Task<ICollection<RawSimpleJoinDataDto>> GetRawReportDataAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}