using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.Contour;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports blood glucose CSV data using the existing Contour parser and persists valid measurements.
    /// </summary>
    public class BloodGlucoseImportHandler : IBloodGlucoseImportHandler
    {
        private readonly IRepository<BloodGlucoseMeasurement> _repository;

        public BloodGlucoseImportHandler(IRepository<BloodGlucoseMeasurement> repository)
        {
            _repository = repository;
        }

        public async Task<ParseResult<BloodGlucoseMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default)
        {
            var result = ContourCsvImporter.ParseCsv(csvStream, hasHeader: hasHeader);

            var data = result.Data?.ToList() ?? new System.Collections.Generic.List<BloodGlucoseMeasurement>();

            if (data.Any())
            {
                foreach (var m in data)
                {
                    await _repository.AddAsync(m, cancellationToken).ConfigureAwait(false);
                }
                await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return result;
        }
    }
}
