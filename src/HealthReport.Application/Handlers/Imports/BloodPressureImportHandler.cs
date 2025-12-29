using HealthReport.Application.FileParsers;
using HealthReport.Application.FileParsers.IHealth;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Imports
{
    /// <summary>
    /// Imports blood pressure CSV data using the existing iHealth parser and persists valid measurements.
    /// </summary>
    public class BloodPressureImportHandler : IBloodPressureImportHandler
    {
        private readonly IRepository<BloodPressureMeasurement> _repository;

        public BloodPressureImportHandler(IRepository<BloodPressureMeasurement> repository)
        {
            _repository = repository;
        }

        public async Task<ParseResult<BloodPressureMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default)
        {
            var result = BloodPressureCsvImporter.ParseCsv(csvStream, hasHeader: hasHeader);

            var data = result.Data?.ToList() ?? new System.Collections.Generic.List<BloodPressureMeasurement>();

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
