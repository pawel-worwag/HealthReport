using HealthReport.Application.FileParsers;

namespace HealthReport.Application.Handlers.Imports;

/// <summary>
/// Marker interface for import handlers.
/// </summary>
public interface IImportHandler
{
    
}

/// <summary>
/// Generic contract for import handlers that parse input streams to a target type.
/// Implementations should return a ParseResult&lt;T&gt; containing parsed Data and any Errors.
/// </summary>
/// <typeparam name="T">Target domain type produced by the importer.</typeparam>
public interface IImportHandler<T> : IImportHandler where T : class
{
    /// <summary>
    /// Import data from the provided stream.
    /// </summary>
    /// <param name="stream">Input stream containing the import data (e.g. CSV).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ParseResult containing parsed items and parse errors.</returns>
    Task<ParseResult<T>> ImportAsync(Stream stream, CancellationToken cancellationToken = default);
}