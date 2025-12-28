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

DEMO Runner: iHealth CSV importer
---------------------------
There is a small runner that demonstrates importing CSV files exported from iHealth.

Run the runner:
```bash
dotnet run --project src/HealthReport.Runners.IHealth.BloodPressureCsvImporter -- "/path/to/BP_Data.csv"
```
The runner expects the CSV file path as the first argument and assumes the file contains a header row by default.
