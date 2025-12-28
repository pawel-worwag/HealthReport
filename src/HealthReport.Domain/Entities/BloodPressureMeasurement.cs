using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace HealthReport.Domain.Entities
{
    public class BloodPressureMeasurement
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PatientId { get; set; }

        public DateOnly MeasuredDate { get; set; }
        public TimeOnly MeasuredTime { get; set; }

        public int? WeekOfYear { get; set; }

        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public int? Pulse { get; set; }

        public string? Note { get; set; }
        public string? Source { get; set; }
    }
}
