using HealthReport.Application.Contracts.Reports;

namespace HealthReport.Application.Handlers.Reports.Simple
{
    public interface ISimpleReportHandler
    {
        Task<SimpleReportDto> GenerateMonthlyReport(int year, int month, Guid userId, CancellationToken cancellationToken = default);
    }
}
