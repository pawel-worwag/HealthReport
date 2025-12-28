# Copilot Instructions — HealthReport

Purpose
-------
Provide an AI assistant with concise, actionable context to work on the HealthReport repository: what it does, where to find key code, how to build/run, and conventions to follow.

Quick summary
-------------
- HealthReport is a .NET 10 solution for collecting, importing and reporting health measurements (blood pressure, etc.).
- Important caller: CSV exporter from iHealth — we provide an importer and a small runner to parse that CSV.

Key locations
-------------
- Solution: `HealthReport.sln`
- Domain entities: `src/HealthReport.Domain/Entities/BloodPressureMeasurement.cs`
- Application services / importers: `src/HealthReport.Application/Services/` (importer lives in namespace `HealthReport.Application.Services.IHealth`)
- CSV runner: `src/HealthReport.Runners.IHealth.BloodPressureCsvImporter/Program.cs`
- Parse result type: `src/HealthReport.Application/Services/ParseResult.cs`

Build & run
-----------
General build:
```bash
dotnet restore
dotnet build HealthReport.sln
```

Run web app (development):
```bash
dotnet run --project src/HealthReport.Web/HealthReport.Web.csproj
```

Run the iHealth CSV runner (demo):
```bash
dotnet run --project src/HealthReport.Runners.IHealth.BloodPressureCsvImporter -- "/path/to/BP.csv"
```
The runner expects a path to the CSV as the first argument and assumes the file contains a header by default.

Conventions & important notes
-----------------------------
- Do not name classes starting with a capital `I` followed by an uppercase letter (that denotes interfaces in C#). For source identifiers like iHealth, prefer using a namespace (e.g. `HealthReport.Application.Services.IHealth`) or suffix/prefix without leading `I` in class name.
- Domain model uses `DateOnly` and `TimeOnly` for measured date/time. If targeting older runtimes, prefer `DateTime` + `TimeSpan` or provide EF converters.
- `WeekOfYear` uses ISO week numbers and should be computed at import time.
- Parser returns `ParseResult<T>` with `Data` and `Errors`; malformed rows are collected in `Errors` (with line numbers) and skipped from `Data`.
- CSV parsing currently uses a small custom splitter; if files become complicated, migrate to a robust library like `CsvHelper`.

Validation guidance
-------------------
- Systolic: 50–250
- Diastolic: 30–150
- Pulse: 30–220
Mark out-of-range values as suspicious or emit parse/validation errors.

Testing & CI
------------
- Add unit tests under a `tests/` project and run via `dotnet test`.
- Keep tasks small and focused; amend last commit only for immediate fixes, and use `--force-with-lease` when pushing rewrites.

Platform decision (ADR)
-----------------------
- This repository targets .NET 10 (`net10.0`). See ADR: `docs/adr/0001-use-dotnet-10.md` for rationale, alternatives and consequences.
- Ensure CI, local SDK and Docker images use .NET 10-compatible tooling (`dotnet --info` should report .NET 10).
