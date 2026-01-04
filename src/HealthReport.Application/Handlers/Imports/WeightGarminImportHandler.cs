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
            var imported = 0;
            foreach (var m in data)
            {
                var exists = repository.Query().Any(x =>
                    x.PatientId == m.PatientId
                    && x.MeasuredDate == m.MeasuredDate
                    && x.MeasuredTime == m.MeasuredTime
                    && x.WeightKg == m.WeightKg
                    && x.WeightChangeKg == m.WeightChangeKg
                    && x.BMI == m.BMI
                    && x.BodyFatPercentage == m.BodyFatPercentage
                    && x.SkeletalMuscleMassKg == m.SkeletalMuscleMassKg
                    && x.BodyWaterPercentage == m.BodyWaterPercentage
                    && x.Source == m.Source
                    && x.SourceDetails == m.SourceDetails);
                if (!exists)
                {
                    await repository.AddAsync(m, cancellationToken).ConfigureAwait(false);
                }
            }

            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new ImportResultDto()
            {
                Imported = imported,
                Errors = result.Errors
            };
        }
    }
}