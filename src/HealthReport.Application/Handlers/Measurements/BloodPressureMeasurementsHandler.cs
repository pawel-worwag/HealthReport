using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements
{
    /// <summary>
    /// Handler that reads blood pressure measurements from repository.
    /// Parameters: patient id and inclusive date range.
    /// Returns DTOs defined in application contracts.
    /// </summary>
    public class BloodPressureMeasurementsHandler(IRepository<BloodPressureMeasurement> repo) : IBloodPressureMeasurementsHandler
    {
        /// <summary>
        /// Get blood pressure measurements for a given patient in the specified date range (inclusive).
        /// Returns DTOs suitable for application layer consumers.
        /// </summary>
        public Task<List<BloodPressureMeasurementDto>> GetForPatientAsync(
            Guid patientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default)
        {
            var query = repo.Query()
                .Where(m => m.PatientId == patientId && m.MeasuredDate >= from && m.MeasuredDate <= to)
                .OrderByDescending(m => m.MeasuredDate)
                .ThenByDescending(m => m.MeasuredTime);

            var list = query.ToList();

            var mapped = list.Select(m => new BloodPressureMeasurementDto
            {
                Id = m.Id,
                MeasuredDate = m.MeasuredDate,
                MeasuredTime = m.MeasuredTime,
                WeekOfYear = m.WeekOfYear,
                Systolic = m.Systolic,
                Diastolic = m.Diastolic,
                Pulse = m.Pulse,
                Note = m.Note,
                Source = m.Source
            }).ToList();

            return Task.FromResult(mapped);
        }
    }
}
