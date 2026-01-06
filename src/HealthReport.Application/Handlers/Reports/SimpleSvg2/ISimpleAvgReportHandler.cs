using HealthReport.Application.Contracts.Reports;

namespace HealthReport.Application.Handlers.Reports.SimpleSvg2;

public interface ISimpleAvgReportHandler
{
    public Task<SimpleAvgReportDto> GenerateReportAsync(DateOnly from, DateOnly to, Guid userId, CancellationToken cancellationToken = default);
}