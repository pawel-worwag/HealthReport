using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Extensions;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Reports.Simple
{
    public class SimpleReportHandler(
        IRepository<BloodPressureMeasurement> bpRepo,
        IRepository<BloodGlucoseMeasurement> bgRepo,
        IRepository<WeightMeasurement> wRepo)
        : ISimpleReportHandler
    {
        public SimpleReportDto GenerateMonthlyReport(int year, int month)
        {
            var from = new DateOnly(year, month, 1);
            var to = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var bpList = bpRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();
            var bgList = bgRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();
            var wList = wRepo.Query().Where(x => x.MeasuredDate >= from && x.MeasuredDate <= to).ToList();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month));

            var records = (from day in days
                select new DateOnly(year, month, day)
                into dateOnly
                let bpEntries = bpList.Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new BloodPressureEntry
                    {
                        Time = x.MeasuredTime,
                        Systolic = x.Systolic,
                        Diastolic = x.Diastolic,
                        Pulse = x.Pulse,
                        Note = x.Note
                    })
                    .ToList()
                let bgEntries = bgList.Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new GlucoseEntry
                        { Time = x.MeasuredTime, Value = x.BGValue, Meal = (int)x.Meal, Note = x.Note })
                    .ToList()
                let wEntries = wList.Where(x => x.MeasuredDate == dateOnly)
                    .OrderBy(x => x.MeasuredTime)
                    .Select(x => new WeightEntry { Time = x.MeasuredTime, WeightKg = x.WeightKg, Bmi = x.BMI })
                    .ToList()
                select new DetailEntry
                    { Date = dateOnly, BloodPressure = bpEntries, Glucose = bgEntries, Weight = wEntries }).ToList();
            
            var systolicMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).MinOrNull();
            var systolicMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).MaxOrNull();
            var systolicAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).AverageOrNull();
            var systolicSummary = new SummaryEntryValue()
                { Min = systolicMin, Max = systolicMax, Avg = (decimal?)systolicAvg };

            var diastolicMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Diastolic)).MinOrNull();
            var diastolicMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Diastolic)).MaxOrNull();
            var diastolicAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Diastolic)).AverageOrNull();
            var diastolicSummary = new SummaryEntryValue()
                { Min = diastolicMin, Max = diastolicMax, Avg = (decimal?)diastolicAvg };

            var pulseMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).MinOrNull();
            var pulseMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).MaxOrNull();
            var pulseAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).AverageOrNull();
            var pulseSummary = new SummaryEntryValue() { Min = pulseMin, Max = pulseMax, Avg = (decimal?)pulseAvg };

            var glucoseMin = records.SelectMany(x => x.Glucose.Select(y => y.Value)).MinOrNull();
            var glucoseMax = records.SelectMany(x => x.Glucose.Select(y => y.Value)).MaxOrNull();
            var glucoseAvg = records.SelectMany(x => x.Glucose.Select(y => y.Value)).AverageOrNull();
            var glucoseSummary = new SummaryEntryValue()
                { Min = glucoseMin, Max = glucoseMax, Avg = (decimal?)glucoseAvg };

            var weightMin = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).MinOrNull();
            var weightMax = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).MaxOrNull();
            var weightAvg = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).AverageOrNull();
            var weightSummary = new SummaryEntryValue() { Min = weightMin, Max = weightMax, Avg = weightAvg };

            var bmiMin = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).MinOrNull();
            var bmiMax = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).MaxOrNull();
            var bmiAvg = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).AverageOrNull();
            var bmiSummary = new SummaryEntryValue() { Min = bmiMin, Max = bmiMax, Avg = bmiAvg };

            return new SimpleReportDto
            {
                Year = year,
                Month = month,
                Details = records,
                Summary = new SummaryEntry()
                {
                    DiastolicSummary = diastolicSummary,
                    SystolicSummary = systolicSummary,
                    PulseSummary = pulseSummary,
                    GlucoseSummary = glucoseSummary,
                    WeightSummary = weightSummary,
                    BmiSummary = bmiSummary
                }
            };
        }
    }
}