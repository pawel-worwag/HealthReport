using HealthReport.Application.FileParsers;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Handler responsible for importing blood pressure measurements from a CSV stream.
    /// </summary>
    public interface IBloodPressureImportHandler
    {
        Task<ParseResult<BloodPressureMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default);
    }
}
