using HealthReport.Application.Contracts.Imports;

namespace HealthReport.Application.Handlers.Imports;

/// <summary>
/// Marker interface for import handlers.
/// </summary>
public interface IImportHandler
{
    Task<ImportResultDto> ImportAsync(Stream stream, bool hasHeader = true,
        CancellationToken cancellationToken = default);
}