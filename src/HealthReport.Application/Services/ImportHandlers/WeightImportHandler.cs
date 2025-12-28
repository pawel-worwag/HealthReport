using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Interfaces;
using HealthReport.Application.Services.Garmin;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Services.ImportHandlers
{
    /// <summary>
    /// Imports weight CSV data using the existing Garmin parser and persists valid measurements.
    /// </summary>
    public class WeightImportHandler : IWeightImportHandler
    {
        private readonly IRepository<WeightMeasurement> _repository;

        public WeightImportHandler(IRepository<WeightMeasurement> repository)
        {
            _repository = repository;
        }

        public async Task<ParseResult<WeightMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true, CancellationToken cancellationToken = default)
        {
            var result = WeightCsvImporter.ParseCsv(csvStream, hasHeader: hasHeader);

            var data = result.Data?.ToList() ?? new System.Collections.Generic.List<WeightMeasurement>();

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
