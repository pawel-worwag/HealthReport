using HealthReport.Application.Contracts.RawReports;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HealthReport.Infrastructure.Repositories;

public class RawReportDataRepository(HealthReportDbContext db)
    : IRawReportDataRepository
{
    public async Task<ICollection<SimpleDataConsolidatedDto>> GetRawReportDataAsync(Guid userId, DateOnly from, DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var bg = await db.BloodGlucoseMeasurements
            .Where(p => p.PatientId == userId && p.MeasuredDate >= from && p.MeasuredDate <= to)
            .ToListAsync(cancellationToken);
        var bp = await db.BloodPressureMeasurements
            .Where(p => p.PatientId == userId && p.MeasuredDate >= from && p.MeasuredDate <= to)
            .ToListAsync(cancellationToken);
        var w = await db.WeightMeasurements
            .Where(p => p.PatientId == userId && p.MeasuredDate >= from && p.MeasuredDate <= to)
            .ToListAsync(cancellationToken);

        var days = new List<DateOnly>();
        for (var i = 0; i < (to.DayNumber - from.DayNumber) + 1; i++)
        {
            days.Add(to.AddDays(-i));
        }

        var result = new List<SimpleDataConsolidatedDto>();
        foreach (var d in days)
        {
            result.Add(new SimpleDataConsolidatedDto()
            {
                Date = d,
                BloodGlucose = bg.Where(p => p.MeasuredDate == d).Select(Map).ToList(),
                BloodPressure = bp.Where(p => p.MeasuredDate == d).Select(Map).ToList(),
                Weight = w.Where(p => p.MeasuredDate == d).Select(Map).ToList(),
            });
        }

        return result;
    }

    private static BloodGlucoseDto Map(BloodGlucoseMeasurement p) => new()
    {
        MeasuredTime = p.MeasuredTime,
        Glucose = p.BGValue,
        Meal = p.Meal,
        Note = p.Note
    };

    private static BloodPressureDto Map(BloodPressureMeasurement p) => new()
    {
        MeasuredTime = p.MeasuredTime,
        Systolic = p.Systolic,
        Diastolic = p.Diastolic,
        Pulse = p.Pulse
    };

    private static WeightDto Map(WeightMeasurement p) => new()
    {
        MeasuredTime = p.MeasuredTime,
        Weight = p.WeightKg,
        Bmi = p.BMI,
        WeightChange = p.WeightChangeKg,
        BodyFatPercentage = p.BodyFatPercentage,
        SkeletalMuscleMass = p.SkeletalMuscleMassKg,
        BodyWaterPercentage = p.BodyWaterPercentage
    };
}