namespace HealthReport.Application.Handlers.Reports.Simple
{
    public interface ISimpleReportHandler
    {
        Task<SimpleReportDto> GenerateMonthlyReportAsync(int year, int month, CancellationToken cancellationToken = default);
    }
}
