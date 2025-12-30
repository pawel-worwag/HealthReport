# Do not use Swagger / Swashbuckle in this repo

Reminder for maintainers and assistants:

- Do NOT add or recommend using `Swashbuckle.AspNetCore` / "Swagger UI" packages in this repository. This project targets .NET 10 and uses the repository's OpenAPI approach (`Microsoft.AspNetCore.OpenApi`, `Scalar.AspNetCore`), which is the supported pattern here.
- Prefer using the existing OpenAPI tooling already used by the project:
  - `builder.Services.AddOpenApi(...)` and `app.MapOpenApi()` (see `Program.cs`).
  - `Scalar.AspNetCore` helpers (project already references `Scalar.AspNetCore`).
- If you need to expose health, metrics or minimal API documentation, create explicit minimal API endpoints with `.MapGet(...)` and annotate them with `.Produces(...)` / `.WithTags(...)` so the OpenAPI generator (used by this project) can pick them up.

If you're unsure, ask before adding a Swagger/Swashbuckle dependency — follow this repository's OpenAPI conventions instead.
