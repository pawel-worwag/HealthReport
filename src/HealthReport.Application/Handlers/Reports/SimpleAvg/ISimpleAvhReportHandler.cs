using HealthReport.Application.Contracts.Reports;

namespace HealthReport.Application.Handlers.Reports.SimpleAvg;

public interface ISimpleAvhReportHandler
{
    SimpleAvgReportDto GenerateReport(int yearFrom, int monthFrom, int yearTo, int monthTo);
}