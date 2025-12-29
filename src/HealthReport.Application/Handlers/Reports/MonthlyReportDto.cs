namespace HealthReport.Application.Handlers.Reports
{
    public class MonthlyReportDto
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public IReadOnlyList<DailyReport> Days { get; init; } = Array.Empty<DailyReport>();
    }

    public class DailyReport
    {
        public DateOnly Date { get; init; }
        public IReadOnlyList<BloodPressureEntry> BloodPressure { get; init; } = Array.Empty<BloodPressureEntry>();
        public IReadOnlyList<GlucoseEntry> Glucose { get; init; } = Array.Empty<GlucoseEntry>();
        public IReadOnlyList<WeightEntry> Weight { get; init; } = Array.Empty<WeightEntry>();
    }

    public class BloodPressureEntry
    {
        public TimeOnly Time { get; init; }
        public int Systolic { get; init; }
        public int Diastolic { get; init; }
        public int? Pulse { get; init; }
        public string? Note { get; init; }
    }

    public class GlucoseEntry
    {
        public TimeOnly Time { get; init; }
        public int Value { get; init; }
        public int Meal { get; init; }
        public string? Note { get; init; }
    }

    public class WeightEntry
    {
        public TimeOnly Time { get; init; }
        public decimal WeightKg { get; init; }
        public decimal? BMI { get; init; }
        public decimal? BodyFatPercentage { get; init; }
    }
}
