using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Handlers.Imports;

/// <summary>
/// Marker interface for import handlers.
/// </summary>
public interface IImportHandler
{
    Task<ImportResultDto> ImportAsync(Stream stream, bool hasHeader = true, CancellationToken cancellationToken = default);
}