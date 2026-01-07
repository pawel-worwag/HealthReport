using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;
using HealthReport.Application.Contracts.Measurements;

namespace HealthReport.Application.Handlers.Measurements
{
    /// <summary>
    /// Handler that reads weight measurements from repository.
    /// Parameters: patient id and inclusive date range.
    /// Returns DTOs defined in application contracts (without PatientId).
    /// </summary>
    public class WeightMeasurementsHandler(IRepository<WeightMeasurement> repo)
    {
        /// <summary>
        /// Get weight measurements for a given patient in the specified date range (inclusive).
        /// </summary>
        public Task<List<WeightMeasurementDto>> GetForPatientAsync(
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

            var mapped = list.Select(m => new WeightMeasurementDto
            {
                Id = m.Id,
                MeasuredDate = m.MeasuredDate,
                MeasuredTime = m.MeasuredTime,
                WeightKg = m.WeightKg,
                WeightChangeKg = m.WeightChangeKg,
                BMI = m.BMI,
                BodyFatPercentage = m.BodyFatPercentage,
                SkeletalMuscleMassKg = m.SkeletalMuscleMassKg,
                BodyWaterPercentage = m.BodyWaterPercentage,
                Source = m.Source,
                SourceDetails = m.SourceDetails,
                Note = m.Note,
                CreatedAt = m.CreatedAt
            }).ToList();

            return Task.FromResult(mapped);
        }
    }
}
