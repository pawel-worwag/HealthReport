using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.Contour;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports blood glucose CSV data using the existing Contour parser and persists valid measurements.
    /// </summary>
    public class BloodGlucoseImportHandler(IRepository<BloodGlucoseMeasurement> repository) : IBloodGlucoseImportHandler
    {
        public async Task<ImportResultDto> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default)
        {
            var result = ContourCsvImporter.ParseCsv(csvStream, hasHeader: hasHeader);

            var data = result.Data?.ToList() ?? [];


            foreach (var m in data)
            {
                await repository.AddAsync(m, cancellationToken);
            }
            await repository.SaveChangesAsync(cancellationToken);
            
            return new ImportResultDto()
            {
                Imported = data.Count,
                Errors = result.Errors
            };
        }
    }
}
