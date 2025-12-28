using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Services;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Services.ImportHandlers
{
    /// <summary>
    /// Handler responsible for importing blood glucose measurements from a CSV stream.
    /// </summary>
    public interface IBloodGlucoseImportHandler
    {
        Task<ParseResult<BloodGlucoseMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default);
    }
}
