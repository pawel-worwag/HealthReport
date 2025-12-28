# HealthReport
# HealthReport

Short description
-----------------
HealthReport is a simple .NET 10 solution for collecting, importing and reporting health measurements (blood pressure, glucose, weight, etc.).

Architecture
------------
- `HealthReport.Domain` — domain entities and business logic.
- `HealthReport.Application` — application services, parsers and use-cases.
- `HealthReport.Infrastructure` — EF Core mappings and data access implementations.
- `HealthReport.Web` — Blazor Server UI and minimal API endpoints.

Technologies
------------
- .NET 10
- Blazor Server

Build & run
-----------
General build:
```bash
dotnet restore
dotnet build HealthReport.sln
```

Run the web app (development):
```bash
dotnet run --project src/HealthReport.Web/HealthReport.Web.csproj
```

Runner: iHealth CSV importer
---------------------------
There is a small runner that demonstrates importing CSV files exported from iHealth.

Run the runner:
```bash
dotnet run --project src/HealthReport.Runners.IHealth.BloodPressureCsvImporter -- "/path/to/BP_Data.csv"
```
The runner expects the CSV file path as the first argument and assumes the file contains a header row by default.

Important files
---------------
- Solution: `HealthReport.sln`
- Domain entity: `src/HealthReport.Domain/Entities/BloodPressureMeasurement.cs`
- CSV importer (iHealth): `src/HealthReport.Application/Services/IHBloodPressureCsvImporter.cs` (namespace `HealthReport.Application.Services.IHealth`, class `BloodPressureCsvImporter`)
- Runner project: `src/HealthReport.Runners.IHealth.BloodPressureCsvImporter/Program.cs`
- Parser result: `src/HealthReport.Application/Services/ParseResult.cs`

Notes & conventions
-------------------
- Avoid naming classes that start with an uppercase `I` followed by another uppercase letter (that's the C# convention for interfaces). For sources like iHealth prefer placing the code in a dedicated namespace (for example `HealthReport.Application.Services.IHealth`) and keep class names without a leading `I`.
- Domain model uses `DateOnly` and `TimeOnly` for measured date/time. If targeting older runtimes, use `DateTime` + `TimeSpan` or add EF Core converters.
- The importer computes `WeekOfYear` (ISO week) at import time and returns a `ParseResult<T>` containing `Data` (parsed records) and `Errors` (parse errors with line numbers).

If you need help
---------------
Provide the failing logs, `dotnet --info`, `dotnet build` output, and a small sample of CSV lines.


Example usage:
```bash
dotnet run --project src/HealthReport.Runners.IHealth.BloodPressureCsvImporter -- "/path/to/file.csv"
```

Notes:
- The runner expects the CSV path as the first argument and assumes the file contains a header row by default.
- Parser: `HealthReport.Application.Services.IHealth.BloodPressureCsvImporter.ParseCsv` returns `ParseResult<T>` with parsed records and parse errors.


