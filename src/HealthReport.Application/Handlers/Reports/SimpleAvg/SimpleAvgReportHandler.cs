using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Extensions;
using HealthReport.Application.Interfaces;

namespace HealthReport.Application.Handlers.Reports.SimpleAvg;

public class SimpleAvgReportHandler(IRawReportDataRepository repo) : ISimpleAvgReportHandler
{
    public async Task<SimpleAvgReportDto> GenerateReportAsync(DateOnly from, DateOnly to, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var data = await repo.GetRawReportDataAsync(userId, from, to, cancellationToken);

        var records = data
            .GroupBy(x => new { Year = x.Date.Year, Month = x.Date.Month })
            .Select(p => new SimpleAvgReportEntry()
            {
                Year = p.Key.Year,
                Month = p.Key.Month,
                Summary = new SummaryEntry()
                {
                    DiastolicSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Diastolic).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Diastolic).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Diastolic).Where(v => v.HasValue).AverageOrNull(),
                    },
                    SystolicSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Systolic).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Systolic).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Systolic).Where(v => v.HasValue).AverageOrNull(),
                    },
                    PulseSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Pulse).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Pulse).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.BloodPressure)
                            .Select(x => x.Pulse).Where(v => v.HasValue).AverageOrNull(),
                    },
                    GlucoseSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.BloodGlucose)
                            .Select(x => x.Glucose).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.BloodGlucose)
                            .Select(x => x.Glucose).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.BloodGlucose)
                            .Select(x => x.Glucose).Where(v => v.HasValue).AverageOrNull(),
                    },
                    WeightSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.Weight)
                            .Select(x => x.Weight).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.Weight)
                            .Select(x => x.Weight).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.Weight)
                            .Select(x => x.Weight).Where(v => v.HasValue).AverageOrNull(), 
                    },
                    BmiSummary = new SummaryEntryValue()
                    {
                        Max = p.SelectMany(g => g.Weight)
                            .Select(x => x.Bmi).Where(v => v.HasValue).MaxOrNull(),
                        Min = p.SelectMany(g => g.Weight)
                            .Select(x => x.Bmi).Where(v => v.HasValue).MinOrNull(),
                        Avg = p.SelectMany(g => g.Weight)
                            .Select(x => x.Bmi).Where(v => v.HasValue).AverageOrNull(), 
                    }
                }
            }).ToList();


        return new SimpleAvgReportDto()
        {
            DateFrom = from.ToString("yyyy-MM"),
            DateTo = to.ToString("yyyy-MM"),
            Records = records
        };
    }
}