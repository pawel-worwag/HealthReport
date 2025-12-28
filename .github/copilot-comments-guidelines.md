# Copilot Comments Guidelines

Purpose
-------
This file instructs Copilot (and other generative assistants) how to produce comments and XML documentation in C# source files within this repository.

Rules (strict)
----------------
- All comments and XML documentation (`/// <summary/>`, `<param/>`, `<returns/>`, `<remarks/>`, etc.) in classes, records, and interfaces must be written in English.
- Prefer plain ASCII for comments. Avoid non-ASCII characters and diacritics (for example: ąćęłńóśżź). If Unicode is required (proper names, library identifiers), keep it minimal and justified.
- Use XML documentation comments for public API (public/internal types and members). Use concise inline comments (`//` or `/* */`) only for implementation notes that are not part of the public contract.
- Do not place long design explanations inside code comments; prefer repository documentation files (e.g. `docs/` or sidecar `*.md`).

Guidance for Copilot prompts
----------------------------
- When asked to generate or modify C# files, output comments and XML docs in English only.
- Use `///` XML comments for public types and members, and include `summary`, `param`, and `returns` where appropriate.
- Keep comments concise and focused; avoid writing paragraphs longer than a few sentences inside code files.
- If asked to include example usage, prefer a short `/// <example>` block or a separate `*.md` example file.

Good examples
-------------
XML doc for a class:

```csharp
/// <summary>
/// Represents a blood pressure measurement.
/// </summary>
public record BloodPressureMeasurement(int Systolic, int Diastolic, int? Pulse);
```

Inline comment:

```csharp
// Use UTC to avoid timezone ambiguities in stored timestamps.
var timestamp = DateTimeOffset.UtcNow;
```

Bad examples (do not generate)
-----------------------------
Contains Polish diacritics — forbidden:

```csharp
/// <summary>
/// Reprezentuje pomiar ciśnienia krwi.
/// </summary>
```

Contains non-ASCII chars — forbidden:

```csharp
// Użyj czasu lokalnego dla czytelności
```

