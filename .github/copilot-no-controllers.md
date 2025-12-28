# Copilot Guideline — Do Not Generate Controllers

Purpose
-------
This guideline tells assistants (Copilot or similar) not to generate new ASP.NET Controller classes in this repository.

Rule (strict)
----------------
- Do NOT generate controller classes (types inheriting from `Controller` / `ControllerBase`) under any project in this repo.

Why
---
- The project follows a pattern where application logic and import handlers live in `HealthReport.Application` and are registered via DI. Creating controllers scatters HTTP concerns across the codebase and bypasses the established handler pattern.
- Handlers are easier to test, reuse, and keep independent of transport (HTTP).

Preferred alternatives
----------------------
- If HTTP exposure is required, either:
  - Add a minimal API route in `Program.cs` that calls a handler from `HealthReport.Application`.
  - Or add a handler/interface in `HealthReport.Application` and register it in DI; let someone else (or a separate minimal endpoint) expose it.

How to ask Copilot
-------------------
- Good prompt: "Create an application handler `IBloodPressureImportHandler` in `HealthReport.Application` that parses a CSV stream and persists measurements. Do not create controller classes; if you need an HTTP endpoint, add a minimal `app.MapPost(...)` in `Program.cs` instead." 
- If asked directly to produce an HTTP endpoint, prefer: "Add a minimal API mapping in `Program.cs` that reads a multipart `file` and calls `IBloodPressureImportHandler.ImportAsync(...)`. Do not generate controllers."

Examples
--------
Bad (forbidden):

```csharp
public class BloodPressureController : ControllerBase
{
    [HttpPost("/api/import/bloodpressure")]
    public IActionResult Import(IFormFile file) { ... }
}
```

Good (preferred):

1) Handler in `HealthReport.Application` (example):

```csharp
public interface IBloodPressureImportHandler { Task<ParseResult<BloodPressureMeasurement>> ImportAsync(Stream csvStream, bool hasHeader = true); }
public class BloodPressureImportHandler : IBloodPressureImportHandler { /* parse + persist */ }
```

2) Minimal API in `Program.cs` (example):

```csharp
app.MapPost("/api/import/bloodpressure", async (HttpRequest request, IBloodPressureImportHandler handler, CancellationToken ct) =>
{
    var form = await request.ReadFormAsync(ct);
    var file = form.Files.GetFile("file");
    using var s = file.OpenReadStream();
    var result = await handler.ImportAsync(s, hasHeader: true, ct);
    return Results.Ok(new { imported = result.Data.Count(), errors = result.Errors });
});
```

