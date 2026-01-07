using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements
{
    public interface IWeightMeasurementsHandler
    {
        Task<List<WeightMeasurementDto>> GetForPatientAsync(Guid patientId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    }
}
