using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements
{
    public interface IBloodPressureMeasurementsHandler
    {
        Task<List<BloodPressureMeasurementDto>> GetForPatientAsync(Guid patientId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    }
}
