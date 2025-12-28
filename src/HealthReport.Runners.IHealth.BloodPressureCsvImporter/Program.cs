using System;
using System.IO;
using System.Linq;
using HealthReport.Application.Services;
using HealthReport.Application.Services.IHealth;

Console.WriteLine("Runners.IHealth.BloodPressureCsvImporter - CSV import demo\n");

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: dotnet run --project HealthReport.Runners.IHealth.BloodPressureCsvImporter -- <path-to-csv>");
    return 1;
}

var path = args[0];
if (!File.Exists(path))
{
    Console.Error.WriteLine($"CSV file not found: {path}");
    return 1;
}

using var fs = File.OpenRead(path);
var result = BloodPressureCsvImporter.ParseCsv(fs, hasHeader: true);

var data = result.Data.ToList();
var errors = result.Errors.ToList();

Console.WriteLine($"Imported records: {data.Count}");
Console.WriteLine($"Parse errors: {errors.Count}\n");

Console.WriteLine("First 10 records:");
foreach (var m in data.Take(10))
{
    Console.WriteLine($"{m.MeasuredDate:yyyy-MM-dd} {m.MeasuredTime:HH:mm}  SYS={m.Systolic}  DIA={m.Diastolic}  Pulse={m.Pulse}");
}

if (errors.Any())
{
    Console.WriteLine("\nErrors:");
    foreach (var e in errors.Take(10))
    {
        Console.WriteLine($"Line {e.Line}: {e.Message}");
    }
}

return 0;
