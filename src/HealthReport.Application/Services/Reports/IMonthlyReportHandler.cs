using System.Threading;
using System.Threading.Tasks;

namespace HealthReport.Application.Services.Reports
{
    public interface IMonthlyReportHandler
    {
        Task<MonthlyReportDto> GenerateMonthlyReportAsync(int year, int month, CancellationToken cancellationToken = default);
    }
}
