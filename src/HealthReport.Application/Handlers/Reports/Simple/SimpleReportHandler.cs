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


            var systolicMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).Min();
            var systolicMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).Max();
            var systolicAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).Average();
            var systolicSummary = new SummaryEntryValue()
                { Min = systolicMin, Max = systolicMax, Avg = (decimal?)systolicAvg };

            var diastolicMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Diastolic)).Min();
            var diastolicMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).Max();
            var diastolicAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Systolic)).Average();
            var diastolicSummary = new SummaryEntryValue()
                { Min = diastolicMin, Max = diastolicMax, Avg = (decimal?)diastolicAvg };

            var pulseMin = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).Min();
            var pulseMax = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).Max();
            var pulseAvg = records.SelectMany(x => x.BloodPressure.Select(y => y.Pulse)).Average();
            var pulseSummary = new SummaryEntryValue() { Min = pulseMin, Max = pulseMax, Avg = (decimal?)pulseAvg };

            var glucoseMin = records.SelectMany(x => x.Glucose.Select(y => y.Value)).Min();
            var glucoseMax = records.SelectMany(x => x.Glucose.Select(y => y.Value)).Max();
            var glucoseAvg = records.SelectMany(x => x.Glucose.Select(y => y.Value)).Average();
            var glucoseSummary = new SummaryEntryValue()
                { Min = glucoseMin, Max = glucoseMax, Avg = (decimal?)glucoseAvg };

            var weightMin = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).Min();
            var weightMax = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).Max();
            var weightAvg = records.SelectMany(x => x.Weight.Select(y => y.WeightKg)).Average();
            var weightSummary = new SummaryEntryValue() { Min = weightMin, Max = weightMax, Avg = weightAvg };

            var bmiMin = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).Min();
            var bmiMax = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).Max();
            var bmiAvg = records.SelectMany(x => x.Weight.Select(y => y.Bmi)).Average();
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