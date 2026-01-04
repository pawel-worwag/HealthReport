using HealthReport.Application.Contracts.Imports;
using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.BloodGlucose.Contour;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports blood glucose CSV data using the existing Contour parser and persists valid measurements.
    /// </summary>
    public class BloodGlucoseContourImportHandler(IRepository<BloodGlucoseMeasurement> repository) : IImportHandler
    {
        public async Task<ImportResultDto> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default)
        {
            var result = CsvParser.ParseCsv(csvStream, hasHeader: hasHeader);

            if (result.Errors.Any())
            {
                return new ImportResultDto()
                {
                    Imported = 0,
                    Duplicates = 0,
                    Errors = result.Errors
                };
            }
            
            var data = result.Data?.ToList() ?? [];
            var imported = 0;
            var duplicates = 0;
            foreach (var m in data)
            {
                var exists = repository.Query().Any(x =>
                    x.PatientId == m.PatientId
                    && x.MeasuredDate == m.MeasuredDate
                    && x.MeasuredTime == m.MeasuredTime
                    && x.BGValue == m.BGValue
                    && x.Meal == m.Meal
                    && x.Note == m.Note);
                if (!exists)
                {
                    await repository.AddAsync(m, cancellationToken);
                    imported++;
                }
                else
                {
                    duplicates++;
                }
            }
            await repository.SaveChangesAsync(cancellationToken);
            
            return new ImportResultDto()
            {
                Imported = imported,
                Duplicates = duplicates,
                Errors = result.Errors
            };
        }
    }
}
