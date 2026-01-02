namespace HealthReport.Application.Handlers.Imports;

/// <summary>
/// Specifies available import sources.
/// Add new values when new importers are implemented.
/// </summary>
public enum ImportSource
{
    IHealthBloodPressure,
    ContourBloodGlucose,
    GarminWeight
}