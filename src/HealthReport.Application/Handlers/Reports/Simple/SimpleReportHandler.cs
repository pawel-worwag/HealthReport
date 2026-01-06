using HealthReport.Application.Contracts.RawReports;
using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Extensions;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;


namespace HealthReport.Application.Handlers.Reports.Simple
{
    ///
    /// TODO: Refactoring required (ugly code)
    /// 
    public class SimpleReportHandler(IRawReportDataRepository repo)
        : ISimpleReportHandler
    {
        public async Task<SimpleReportDto> GenerateMonthlyReport(int year, int month, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var from = new DateOnly(year, month, 1);
            var to = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var data = await repo.GetRawReportDataAsync(userId, from, to, cancellationToken);


            return new SimpleReportDto()
            {
                Year = year,
                Month = month,
                Details = data.Select(x => new DetailEntry()
                {
                    Date = x.Date,
                    BloodPressure = x.BloodPressure.OrderBy(p => p.MeasuredTime).Select(Map).ToList(),
                    Glucose = x.BloodGlucose.OrderBy(p => p.MeasuredTime).Select(Map).ToList(),
                    Weight = x.Weight.OrderBy(p => p.MeasuredTime).Select(Map).ToList(),
                }).ToList(),
                Summary = new SummaryEntry()
                {
                    SystolicSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x => x.BloodPressure).Select(p => p.Systolic).MinOrNull(),
                        Max = data.SelectMany(x => x.BloodPressure).Select(p => p.Systolic).MaxOrNull(),
                        Avg = data.SelectMany(x => x.BloodPressure).Select(p => p.Systolic).AverageOrNull()
                    },
                    DiastolicSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x => x.BloodPressure).Select(p => p.Diastolic).MinOrNull(),
                        Max = data.SelectMany(x => x.BloodPressure).Select(p => p.Diastolic).MaxOrNull(),
                        Avg = data.SelectMany(x => x.BloodPressure).Select(p => p.Diastolic).AverageOrNull()
                    },
                    PulseSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x => x.BloodPressure).Select(p => p.Pulse).MinOrNull(),
                        Max = data.SelectMany(x => x.BloodPressure).Select(p => p.Pulse).MaxOrNull(),
                        Avg = data.SelectMany(x => x.BloodPressure).Select(p => p.Pulse).AverageOrNull()
                    },
                    GlucoseSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x=>x.BloodGlucose).Select(p=>p.Glucose).MinOrNull(),
                        Max = data.SelectMany(x=>x.BloodGlucose).Select(p=>p.Glucose).MaxOrNull(),
                        Avg = data.SelectMany(x=>x.BloodGlucose).Select(p=>p.Glucose).AverageOrNull()
                    },
                    WeightSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x=>x.Weight).Select(p=>p.Weight).MinOrNull(),
                        Max = data.SelectMany(x=>x.Weight).Select(p=>p.Weight).MaxOrNull(),
                        Avg = data.SelectMany(x=>x.Weight).Select(p=>p.Weight).AverageOrNull()
                    },
                    BmiSummary = new SummaryEntryValue()
                    {
                        Min = data.SelectMany(x=>x.Weight).Select(p=>p.Bmi).MinOrNull(),
                        Max = data.SelectMany(x=>x.Weight).Select(p=>p.Bmi).MaxOrNull(),
                        Avg = data.SelectMany(x=>x.Weight).Select(p=>p.Bmi).AverageOrNull()
                    }
                }
            };
        }

        private static BloodPressureEntry Map(BloodPressureDto p) => new BloodPressureEntry
        {
            Time = p.MeasuredTime.GetValueOrDefault(),
            Systolic = (int)p.Systolic.GetValueOrDefault(),
            Diastolic = (int)p.Diastolic.GetValueOrDefault(),
            Pulse = p.Pulse
        };


        /// TODO: add Note property
        private static GlucoseEntry Map(BloodGlucoseDto p) => new GlucoseEntry()
        {
            Time = p.MeasuredTime.GetValueOrDefault(),
            Value = p.Glucose.GetValueOrDefault(),
            Meal = (int)p.Meal.GetValueOrDefault()
        };

        private static WeightEntry Map(WeightDto p) => new WeightEntry()
        {
            Time = p.MeasuredTime.GetValueOrDefault(),
            WeightKg = p.Weight.GetValueOrDefault(),
            Bmi = p.Bmi
        };
    }
}