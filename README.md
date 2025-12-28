# HealthReport

Krótki opis:

- Cel: aplikacja do tworzenia raportów pomiarów zdrowotnych (ciśnienie, glukoza, waga itp.).
- Nazwa projektu: **HealthReport**

Architektura:

- `HealthReport.Domain` – modele domenowe i wartość biznesowa.
- `HealthReport.Application` – logika aplikacji, DTO i interfejsy (use-cases).
- `HealthReport.Infrastructure` – implementacje (EF Core, repozytoria, konfiguracje dostępu do danych).
- `HealthReport.Web` – interfejs użytkownika (Blazor Server) i minimalna konfiguracja API.

Technologie:

- .NET 10
- Blazor Server
- Minimal API

Jak uruchomić (lokalnie):

```bash
dotnet restore
dotnet build HealthReport.sln
dotnet run --project src/HealthReport.Web/HealthReport.Web.csproj
```

Pliki warte uwagi:

- [HealthReport.sln](HealthReport.sln#L1)
- [src/HealthReport.Web/Program.cs](src/HealthReport.Web/Program.cs#L1)

DEMO Runner (iHealth CSV importer)

- Project: `src/HealthReport.Runners.IHealth.BloodPressureCsvImporter`
- Purpose: simple runner that parses a CSV exported from iHealth and prints few of imported `BloodPressureMeasurement` records.

Example usage:
```bash
dotnet run --project src/HealthReport.Runners.IHealth.BloodPressureCsvImporter -- "/path/to/file.csv"
```

Notes:
- The runner expects the CSV path as the first argument and assumes the file contains a header row by default.
- Parser: `HealthReport.Application.Services.IHealth.BloodPressureCsvImporter.ParseCsv` returns `ParseResult<T>` with parsed records and parse errors.


