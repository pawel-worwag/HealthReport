using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.Weight.Garmin;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports weight CSV data using the existing Garmin parser and persists valid measurements.
    /// </summary>
    public class WeightGarminImportHandler(IRepository<WeightMeasurement> repository) : IImportHandler
    {
        public async Task<ImportResultDto> ImportAsync(Stream csvStream, bool hasHeader = true,
            CancellationToken cancellationToken = default)
        {
            var result = CsvParser.ParseCsv(csvStream, hasHeader: hasHeader);

            if (result.Errors.Any())
            {
                return new ImportResultDto()
                {
                    Imported = 0,
                    Errors = result.Errors
                };
            }
            
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