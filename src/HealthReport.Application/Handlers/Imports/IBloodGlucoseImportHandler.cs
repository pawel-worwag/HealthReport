using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Handler responsible for importing blood glucose measurements from a CSV stream.
    /// </summary>
    public interface IBloodGlucoseImportHandler
    {
        Task<ImportResultDto> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default);
    }
}
