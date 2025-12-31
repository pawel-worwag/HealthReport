using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.Garmin;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports weight CSV data using the existing Garmin parser and persists valid measurements.
    /// </summary>
    public class WeightImportHandler(IRepository<WeightMeasurement> repository) : IWeightImportHandler
    {
        public async Task<ImportResultDto> ImportAsync(Stream csvStream, bool hasHeader = true,
            CancellationToken cancellationToken = default)
        {
            var result = WeightCsvImporter.ParseCsv(csvStream, hasHeader: hasHeader);

            var data = result.Data?.ToList() ?? [];

            foreach (var m in data)
            {
                await repository.AddAsync(m, cancellationToken).ConfigureAwait(false);
            }

            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ImportResultDto()
            {
                Imported = data.Count,
                Errors = result.Errors
            };
        }
    }
}