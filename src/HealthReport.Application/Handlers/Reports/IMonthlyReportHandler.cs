namespace HealthReport.Application.Handlers.Reports
{
    public interface IMonthlyReportHandler
    {
        Task<MonthlyReportDto> GenerateMonthlyReportAsync(int year, int month, CancellationToken cancellationToken = default);
    }
}
