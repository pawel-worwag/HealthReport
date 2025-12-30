namespace HealthReport.Application.Handlers.Reports.Simple
{
    public interface ISimpleReportHandler
    {
        SimpleReportDto GenerateMonthlyReport(int year, int month);
    }
}
