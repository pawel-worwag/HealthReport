using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements
{
    /// <summary>
    /// Handler that reads blood glucose measurements from repository.
    /// Parameters: patient id and inclusive date range.
    /// Returns DTOs defined in application contracts (without PatientId).
    /// </summary>
    public class BloodGlucoseMeasurementsHandler(IRepository<BloodGlucoseMeasurement> repo) : IBloodGlucoseMeasurementsHandler
    {
        /// <summary>
        /// Get blood glucose measurements for a given patient in the specified date range (inclusive).
        /// </summary>
        public Task<List<BloodGlucoseMeasurementDto>> GetForPatientAsync(
            Guid patientId,
            DateOnly from,
            DateOnly to,
            CancellationToken cancellationToken = default)
        {
            var query = repo.Query()
                .Where(m => m.PatientId == patientId && m.MeasuredDate >= from && m.MeasuredDate <= to)
                .OrderBy(m => m.MeasuredDate)
                .ThenBy(m => m.MeasuredTime);

            var list = query.ToList();

            var mapped = list.Select(m => new BloodGlucoseMeasurementDto
            {
                Id = m.Id,
                MeasuredDate = m.MeasuredDate,
                MeasuredTime = m.MeasuredTime,
                Glucose = m.BGValue,
                Meal = m.Meal,
                Note = m.Note,
                CreatedAt = m.CreatedAt
            }).ToList();

            return Task.FromResult(mapped);
        }
    }
}
