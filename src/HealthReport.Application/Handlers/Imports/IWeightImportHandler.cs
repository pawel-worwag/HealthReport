
using HealthReport.Application.FileParsers;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Handler responsible for importing weight measurements from a CSV stream.
    /// </summary>
    public interface IWeightImportHandler
    {
        Task<ParseResult<WeightMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default);
    }
}
