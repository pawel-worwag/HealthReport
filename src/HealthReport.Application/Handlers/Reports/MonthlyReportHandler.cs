using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Services.Reports
{
    public class MonthlyReportHandler(
        IRepository<BloodPressureMeasurement> bpRepo,
        IRepository<BloodGlucoseMeasurement> bgRepo,
        IRepository<WeightMeasurement> wRepo)
        : IMonthlyReportHandler
    {
        public async Task<MonthlyReportDto> GenerateMonthlyReportAsync(int year, int month, CancellationToken cancellationToken = default)
        {

            var from = new DateOnly(year, month, 1);
            var to = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
            
            var bpList = bpRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();
            var bgList = bgRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();
            var wList = wRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();

            var records = new List<DailyReport>();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month));
            foreach (var day in days)
            {
                var dateOnly = new DateOnly(year, month, day);
                var bpEntries = bpList
                    .Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new BloodPressureEntry
                    {
                        Time = x.MeasuredTime.ToTimeSpan(),
                        Systolic = x.Systolic,
                        Diastolic = x.Diastolic,
                        Pulse = x.Pulse,
                        Note = x.Note
                    })
                    .ToList();
                var bgEntries = bgList
                    .Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new GlucoseEntry
                    {
                        Time = x.MeasuredTime.ToTimeSpan(),
                        Value = x.BGValue,
                        Meal = (int)x.Meal,
                        Note = x.Note
                    })
                    .ToList();
                var wEntries = wList
                    .Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new WeightEntry
                    {
                        Time = x.MeasuredTime.ToTimeSpan(),
                        WeightKg = x.WeightKg,
                        BMI = x.BMI,
                        BodyFatPercentage = x.BodyFatPercentage
                    })
                    .ToList();
                records.Add(new DailyReport
                {
                    Date = dateOnly,
                    BloodPressure = bpEntries,
                    Glucose = bgEntries,
                    Weight = wEntries
                });
            }
            
            return new MonthlyReportDto
            {
                Year = year,
                Month = month,
                Days = records
            };


        }
    }
}
