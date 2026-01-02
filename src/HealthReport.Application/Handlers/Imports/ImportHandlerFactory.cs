namespace HealthReport.Application.Handlers.Imports;

/// <summary>
/// Simple factory that returns a handler based on ImportSource using a switch.
/// Handler instances are resolved from IServiceProvider so DI lifetimes are respected.
/// </summary>
public class ImportHandlerFactory(IServiceProvider provider)
{
    /// <summary>
    /// Resolve a handler for the given import source.
    /// Replace Concrete* types with actual handler types and ensure they are registered in DI.
    /// </summary>
    public IImportHandler Create(ImportSource source)
    {
        Type handlerType = source switch
        {
            ImportSource.IHealthBloodPressure => typeof(IBloodPressureImportHandler),   // replace with real type
            ImportSource.ContourBloodGlucose => typeof(IBloodGlucoseImportHandler),   // replace with real type
            ImportSource.GarminWeight => typeof(IWeightImportHandler),                 // replace with real type
            _ => throw new InvalidOperationException($"No handler configured for import source: {source}")
        };

        var svc = provider.GetService(handlerType)
                  ?? throw new InvalidOperationException($"Service {handlerType} not registered in DI");

        return svc as IImportHandler
               ?? throw new InvalidOperationException($"Resolved service {handlerType} does not implement IImportHandler");
    }
}