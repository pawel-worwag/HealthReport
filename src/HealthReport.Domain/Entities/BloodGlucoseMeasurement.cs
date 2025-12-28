using System;
using System.ComponentModel.DataAnnotations;

namespace HealthReport.Domain.Entities
{
    /// <summary>
    /// Represents a blood glucose measurement exported from a Contour device.
    /// Date and time are stored separately using DateOnly and TimeOnly.
    /// </summary>
    public enum MealMarker
    {
        Unknown = 0,
        Fasting = 1,
        BeforeMeal = 2,
        AfterMeal = 3
    }

    public class BloodGlucoseMeasurement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? PatientId { get; set; }

        // Date and time stored separately
        public DateOnly MeasuredDate { get; set; }
        public TimeOnly MeasuredTime { get; set; }

        // Blood glucose value in mg/dL
        [Range(10, 1000)]
        public int BGValue { get; set; }

        // Meal marker (if available)
        public MealMarker Meal { get; set; } = MealMarker.Unknown;

        // Optional notes from device/user
        public string? Note { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
