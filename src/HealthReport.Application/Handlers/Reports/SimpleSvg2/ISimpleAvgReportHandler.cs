using HealthReport.Application.Contracts.Reports;

namespace HealthReport.Application.Handlers.Reports.SimpleSvg2;

public interface ISimpleAvgReportHandler
{
    SimpleAvgReportDto GenerateReport(int yearFrom, int monthFrom, int yearTo, int monthTo, Guid userId);
}