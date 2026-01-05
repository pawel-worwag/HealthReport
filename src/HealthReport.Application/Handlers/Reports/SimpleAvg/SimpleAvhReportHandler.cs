using HealthReport.Application.Contracts.Reports;
using HealthReport.Application.Extensions;
using HealthReport.Application.Interfaces;
using HealthReport.Domain.Entities;

namespace HealthReport.Application.Handlers.Reports.SimpleAvg;

public class SimpleAvhReportHandler (
    IRepository<BloodPressureMeasurement> bpRepo,
    IRepository<BloodGlucoseMeasurement> bgRepo,
    IRepository<WeightMeasurement> wRepo)
    :ISimpleAvhReportHandler
{
    public SimpleAvgReportDto GenerateReport(int yearFrom, int monthFrom, int yearTo, int monthTo, Guid userId)
    {
        var from = new DateOnly(yearFrom, monthFrom, 1);
        var to = new DateOnly(yearTo, monthTo, DateTime.DaysInMonth(yearTo, monthTo));
        if (to < from)
        {
            throw new ArgumentException("'To' date must be after 'from' date");
        }

        var bpList = bpRepo.Query().Where(x => x.PatientId == userId && x.MeasuredDate >= from && x.MeasuredDate <= to)
            .GroupBy(x => new { x.MeasuredDate.Year, x.MeasuredDate.Month }).ToList()
            .Select(g =>
                new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SystolicMin = g.MinOrNull(x => x.Systolic),
                    SystolicMax = g.MaxOrNull(x => x.Systolic),
                    SystolicAvg = g.AverageOrNull(x => x.Systolic),
                    DiastolicMin = g.MinOrNull(x => x.Diastolic),
                    DiastolicMax = g.MaxOrNull(x => x.Diastolic),
                    DiastolicAvg = g.AverageOrNull(x => x.Diastolic),
                    PulseMin = g.Where(x=>x.Pulse.HasValue).MinOrNull(x => (int)(x.Pulse??0)),
                    PulseMax = g.Where(x=>x.Pulse.HasValue).MaxOrNull(x => (int)(x.Pulse??0)),
                    PulseAvg = g.Where(x=>x.Pulse.HasValue).AverageOrNull(x => (int)(x.Pulse??0))
                }).ToList();;

        var bgList = bgRepo.Query().Where(x => x.PatientId == userId && x.MeasuredDate >= from && x.MeasuredDate <= to)
            .GroupBy(x => new { x.MeasuredDate.Year, x.MeasuredDate.Month }).ToList()
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                GlucoseMin = g.MinOrNull(x => x.BGValue),
                GlucoseMax = g.MaxOrNull(x => x.BGValue),
                GlucoseAvg = g.AverageOrNull(x => x.BGValue)
            }).ToList();;

        var wList = wRepo.Query().Where(x => x.PatientId == userId && x.MeasuredDate >= from && x.MeasuredDate <= to)
            .GroupBy(x => new { x.MeasuredDate.Year, x.MeasuredDate.Month }).ToList()
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                WeightMin = g.MinOrNull(x => x.WeightKg),
                WeightMax = g.MaxOrNull(x => x.WeightKg),
                WeightAvg = g.AverageOrNull(x => x.WeightKg),
                BmiMin = g.Min(x => x.BMI),
                BmiMax = g.Max(x => x.BMI),
                BmiAvg = g.Average(x => x.BMI)
            }).ToList();

        var months = MonthsRange(yearFrom, monthFrom, yearTo, monthTo).ToList();

        ICollection<SimpleAvgReportEntry> records = new List<SimpleAvgReportEntry>();
        foreach (var m in months)
        {
            var b = bpList.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
            var g = bgList.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
            var w = wList.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
            
            records.Add(new SimpleAvgReportEntry()
            {
                Year = m.Year,
                Month = m.Month,
                Summary = new SummaryEntry()
                {
                    DiastolicSummary = new SummaryEntryValue()
                    {
                        Min = b?.DiastolicMin,
                        Max = b?.DiastolicMax,
                        Avg = b?.DiastolicAvg
                    },
                    SystolicSummary = new SummaryEntryValue()
                    {
                        Min = b?.SystolicMin,
                        Max = b?.SystolicMax,
                        Avg = b?.SystolicAvg
                    },
                    PulseSummary = new SummaryEntryValue()
                    {
                        Min = b?.PulseMin,
                        Max = b?.PulseMax,
                        Avg = b?.PulseAvg
                    },
                    GlucoseSummary = new SummaryEntryValue()
                    {
                        Min = g?.GlucoseMin,
                        Max = g?.GlucoseMax,
                        Avg = g?.GlucoseAvg
                    },
                    WeightSummary = new SummaryEntryValue()
                    {
                        Min = w?.WeightMin,
                        Max = w?.WeightMax,
                        Avg = w?.WeightAvg
                    },
                    BmiSummary = new SummaryEntryValue()
                    {
                        Min = w?.BmiMin,
                        Max = w?.BmiMax,
                        Avg = w?.BmiAvg
                    }
                }
            });
        }

        return new SimpleAvgReportDto()
        {
            DateFrom = from.ToString("yyyy-MM"),
            DateTo = to.ToString("yyyy-MM"),
            Records = records
        };
    }
    
    private IEnumerable<DateOnly> MonthsRange(int yearFrom, int monthFrom, int yearTo, int monthTo)
    {
        var start = new DateOnly(yearFrom, monthFrom, 1);
        var end = new DateOnly(yearTo, monthTo, 1);
        if (end < start) yield break;
        
        var months = (end.Year - start.Year) * 12 + end.Month - start.Month;
        for (var i = 0; i <= months; i++)
            yield return end.AddMonths(-i);
    }
}